# Combat VFX

在命中点实例化 `VFX_EnemyHit`。这是一个面向相机、播放一次并自动销毁的金色火花。

挥刀时可以实例化 `VFX_Slash_Normal`（第 1–3 段，暖金色）或 `VFX_Slash_Electric`（第 4 段，青蓝色）。刀光是六帧弧形粒子，形状随 Prefab 的旋转调整，可将本地 XY 平面对齐挥刀平面。两个刀光共用同一贴图，颜色可在各自的 Material 里调整。

贴图取自 [SpriteCanvas Free Effect Sprite Sheets](https://spritecanvas.com/free-sprite-sheets/effects/)，授权为 CC0 1.0。发布方说明贴图通过 AI 工作流生成，原始许可见 `SpriteCanvas-LICENSE.txt`。

