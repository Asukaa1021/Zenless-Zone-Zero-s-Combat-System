# 安比资源重新导出记录

日期：2026-11-16。目标 Unity：2022.3.62f3，Built-in 渲染管线。

## 交付内容

- 1 个带蒙皮模型：`Model/Anbi.fbx`。
- 45 个动画，分为待机、走路、跑步及掉头、攻击、闪避、弹反、受击与死亡、切人、其他。
- 2 个文件不同的源场景版本：`Variants/ModifiedCopy` 中的 Walk、Run_Start。两份 MAX 文件字节不同，不代表实际动作一定不同，因此分别保留。
- 10 张原始 TGA 贴图：`_D` 为颜色，`_N` 为法线，`_M` 为游戏用遮罩。遮罩设为线性数据，未擅自按 Standard 金属度贴图解释。
- `Materials`：4 个使用颜色贴图的 Standard 基础预览材质及地面材质。这些材质用于查看模型，不等同于游戏的卡通渲染效果。
- `Preview/Anbi_AnimationPreview.unity`：Idle、Walk、Run 三个动画的独立对照场景。
- `Preview/Anbi_Reexport.prefab`：使用新模型、新 Avatar 与待机控制器的预览预制体。
- `export-manifest.csv`：完整动画分类、时长、SHA256 和源文件校验表。

## 使用方法

打开项目中的 `Assets/Anbi_Reexport_20261116/Preview/Anbi_AnimationPreview.unity`，进入 Play 模式观察三个动作。原场景、原 Animator 控制器和角色脚本没有被替换。

预览场景只演示动画播放，不含移动输入、CharacterController、攻击判定或战斗状态机。确认预览正常后，再逐步把新动画接到现有角色。

## 源文件与导出

基准使用 `D:/新建文件夹/anbi/anbi/anbi/max` 的 46 个 MAX 场景；同时保留外层 `D:/新建文件夹/anbi/anbi/max` 中两个不同文件的导出版本。没有保存或覆盖任何源 MAX 文件，导出前后逐文件核对了 SHA256。

使用已安装的 3ds Max 2027.2 Batch 与 FBX 2020 导出器。所有动画按完整源时间区间以 60 fps 烘焙，不压缩曲线；模型与动画骨骼层级一致。模型包含蒙皮、混合形状与颜色贴图；动画 FBX 不重复携带网格几何体。

旧场景有两个缺失的历史插件条目：ViewportManager 和 mental ray 材质附加属性。骨骼、蒙皮网格与标准材质可以读取。没有还原这些旧插件的专属渲染属性。

## Unity 导入设置

- Scale Factor：100，保留文件单位。导入后模型包围盒高度约 1.617 米。
- Rig：Generic；主模型生成 Avatar，所有动画复制主模型 Avatar；Root 为 Bip001。
- 不优化隐藏骨骼，不压缩动画曲线，保留 60 fps。
- Idle、Idle_AFK、Walk、Run 开启 Loop Time，其余动作不循环。
- Root Transform Rotation 和 Position Y 使用 Bake Into Pose；Position XZ 保留根位移。

这样可保留当前动作的身体旋转与上下起伏，并将平面根位移提供给之后的 CharacterController 接入。转身动作的角色朝向、空中位移和攻击突进仍需配合玩法逻辑设计；当前没有修改这些玩法代码。

## 验证范围

- 48 个 FBX 的结构、源文件哈希、输出哈希和动画时长通过检查。
- Unity 2022.3.62f3 中主模型 Avatar 有效，包含 5 个蒙皮网格、24350 个顶点和 151 个 Transform。
- 47 个动画在独立检查工程中完成逐帧采样，共 5677 个采样点；模拟了只读取 OnAnimatorMove、不应用根变换的情况。
- 新 Walk、Run 测试中保持正常身体朝向，没有出现整个人横躺。该结果不能单独证明旧动画被改坏，也不能替代在原角色层级上的最终运行检查。
- `Anbi-preview.png` 为 Unity 实际渲染图。所有动画已做导入与数据采样；没有声称人工逐个观看了全部攻击、闪避和受击动作。

工作目录中的 `UnityAssets` 是原始 FBX 导出；`ValidationProject/Assets/Anbi_Reexport_20261116` 是带 Unity 导入元数据、材质和预览场景的完整交付目录。`Anbi_Reexport_20261116.unitypackage` 可用于备份或另一个 Built-in 项目。

原素材权利归原权利人所有，本次仅整理用户提供的学习素材，未添加新的授权声明。
