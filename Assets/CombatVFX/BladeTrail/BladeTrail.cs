using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
[DefaultExecutionOrder(100)]
public sealed class BladeTrail : MonoBehaviour
{
    [SerializeField] private Transform bladeRoot;
    [SerializeField] private Transform bladeTip;
    [SerializeField] private Material trailMaterial;
    [SerializeField] private Color normalColor = new Color32(255, 195, 108, 255);
    [SerializeField] private Color electricColor = new Color32(69, 217, 255, 255);
    [SerializeField, Range(0.05f, 0.5f)] private float lifetime = 0.17f;
    [SerializeField, Min(0.05f)] private float emissionTimeout = 0.4f;

    private const int MaxSamples = 96;
    private const float MinMovementSquared = 0.000004f;

    private struct Sample
    {
        public Vector3 Root;
        public Vector3 Tip;
        public float Time;
        public Color Color;
        public int Swing;
    }

    private readonly List<Sample> samples = new List<Sample>(MaxSamples);
    private readonly List<Vector3> vertices = new List<Vector3>(MaxSamples * 2);
    private readonly List<Vector2> uvs = new List<Vector2>(MaxSamples * 2);
    private readonly List<Color> colors = new List<Color>(MaxSamples * 2);
    private readonly List<int> triangles = new List<int>((MaxSamples - 1) * 6);

    private Mesh mesh;
    private MeshRenderer trailRenderer;
    private Color currentColor;
    private int swing;
    private float emitUntil;

    private void Awake()
    {
        if (trailMaterial == null)
            return;

        var visual = new GameObject("Blade Trail Mesh");
        visual.layer = gameObject.layer;
        visual.transform.SetParent(transform, false);

        mesh = new Mesh { name = "Blade Trail" };
        mesh.MarkDynamic();
        visual.AddComponent<MeshFilter>().sharedMesh = mesh;
        trailRenderer = visual.AddComponent<MeshRenderer>();
        trailRenderer.sharedMaterial = trailMaterial;
        trailRenderer.shadowCastingMode = ShadowCastingMode.Off;
        trailRenderer.receiveShadows = false;
        trailRenderer.enabled = false;
    }

    // Animation Event: 1-3 are normal attacks, 4 is the electric finisher.
    public void BeginTrail(int attackStep)
    {
        if (attackStep < 1 || attackStep > 4)
            return;

        swing++;
        currentColor = attackStep == 4 ? electricColor : normalColor;
        emitUntil = Time.time + emissionTimeout;
    }

    public void EndTrail()
    {
        emitUntil = 0f;
    }

    private void LateUpdate()
    {
        UpdateTrail(Time.time);
    }

    private void UpdateTrail(float now)
    {
        if (mesh == null)
            return;

        int expired = 0;
        while (expired < samples.Count && now - samples[expired].Time >= lifetime)
            expired++;
        if (expired > 0)
            samples.RemoveRange(0, expired);

        if (now < emitUntil && bladeRoot != null && bladeTip != null)
            RecordBlade(now);

        if (samples.Count >= 2)
            UpdateMesh(now);
        else
            trailRenderer.enabled = false;
    }

    private void RecordBlade(float now)
    {
        Vector3 root = bladeRoot.position;
        Vector3 tip = bladeTip.position;
        if (samples.Count == 0 || samples[samples.Count - 1].Swing != swing)
        {
            AddSample(root, tip, now);
            return;
        }

        Sample previous = samples[samples.Count - 1];
        float movement = Mathf.Max((root - previous.Root).sqrMagnitude, (tip - previous.Tip).sqrMagnitude);
        if (movement < MinMovementSquared)
            return;

        // Start a new strip if the character is teleported.
        if (movement > 6.25f)
        {
            swing++;
            AddSample(root, tip, now);
            return;
        }

        Vector3 previousBlade = previous.Tip - previous.Root;
        Vector3 currentBlade = tip - root;
        int segments = Mathf.Clamp(Mathf.CeilToInt(Vector3.Angle(previousBlade, currentBlade) / 12f), 1, 8);
        for (int segment = 1; segment < segments; segment++)
        {
            float progress = (float)segment / segments;
            Vector3 segmentRoot = Vector3.Lerp(previous.Root, root, progress);
            Vector3 segmentTip = segmentRoot + Vector3.Slerp(previousBlade, currentBlade, progress);
            AddSample(segmentRoot, segmentTip, Mathf.Lerp(previous.Time, now, progress));
        }
        AddSample(root, tip, now);
    }

    private void AddSample(Vector3 root, Vector3 tip, float time)
    {
        if (samples.Count == MaxSamples)
            samples.RemoveAt(0);

        samples.Add(new Sample { Root = root, Tip = tip, Time = time, Color = currentColor, Swing = swing });
    }

    private void UpdateMesh(float now)
    {
        vertices.Clear();
        uvs.Clear();
        colors.Clear();
        triangles.Clear();

        Matrix4x4 toLocal = trailRenderer.transform.worldToLocalMatrix;
        for (int index = 0; index < samples.Count; index++)
        {
            Sample sample = samples[index];
            float age = Mathf.Clamp01((now - sample.Time) / lifetime);
            float opacity = 1f - age * age;
            Color color = sample.Color;
            color.a *= opacity;

            vertices.Add(toLocal.MultiplyPoint3x4(sample.Root));
            vertices.Add(toLocal.MultiplyPoint3x4(sample.Tip));
            uvs.Add(new Vector2(0f, 0f));
            uvs.Add(new Vector2(0f, 1f));
            colors.Add(color);
            colors.Add(color);

            if (index == 0 || samples[index - 1].Swing != sample.Swing)
                continue;

            int vertex = index * 2;
            triangles.Add(vertex - 2);
            triangles.Add(vertex - 1);
            triangles.Add(vertex);
            triangles.Add(vertex);
            triangles.Add(vertex - 1);
            triangles.Add(vertex + 1);
        }

        mesh.Clear(false);
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetColors(colors);
        mesh.SetTriangles(triangles, 0);
        trailRenderer.enabled = triangles.Count > 0;
    }

    private void OnDisable()
    {
        EndTrail();
        samples.Clear();
        if (trailRenderer != null)
            trailRenderer.enabled = false;
    }

    private void OnDestroy()
    {
        if (trailRenderer != null)
            Destroy(trailRenderer.gameObject);
        if (mesh != null)
            Destroy(mesh);
    }

    private void OnDrawGizmosSelected()
    {
        if (bladeRoot == null || bladeTip == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(bladeRoot.position, bladeTip.position);
        Gizmos.DrawWireSphere(bladeRoot.position, 0.025f);
        Gizmos.DrawWireSphere(bladeTip.position, 0.025f);
    }
}

