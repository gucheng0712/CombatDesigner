using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Model Component Attribute
[IncludeMyAttributes]
[TitleGroup("Basic")]
[PropertyOrder(0)]
[ShowInInspector]
public class ModelBasic : Attribute
{

}

[IncludeMyAttributes]
[ShowIf("showReadOnly")]
[TitleGroup("Component Data")]
[PropertyOrder(0)]
[ShowInInspector]
[ReadOnly]
public class ModelComponentReadOnly : Attribute
{

}

#endregion


#region Model Physics Attribute
[IncludeMyAttributes]
[TitleGroup("Physical Property Data")]
[PropertyOrder(1)]
[ShowInInspector]
public class ModelPhysics : Attribute
{

}

[IncludeMyAttributes]
[ShowIf("showReadOnly")]
[TitleGroup("Physical Property Data")]
[PropertyOrder(1)]
[ShowInInspector]
[ReadOnly]
public class ModelPhysicsReadOnly : Attribute
{

}
#endregion


#region Model Status Attribute
[IncludeMyAttributes]
[TitleGroup("Status Data")]
[ShowInInspector]
[PropertyOrder(2)]
public class ModelStatus : Attribute
{

}

[IncludeMyAttributes]
[ShowIf("showReadOnly")]
[TitleGroup("Status Data")]
[PropertyOrder(2)]
[ShowInInspector]
[ReadOnly]
public class ModelStatusReadOnly : Attribute
{

}
#endregion

#region Model Behavior Attribute
[IncludeMyAttributes]
[TitleGroup("Behavior Data")]
[PropertyOrder(3)]
[ShowInInspector]
public class ModelBehavior : Attribute
{

}
[IncludeMyAttributes]
[ShowIf("showReadOnly")]
[TitleGroup("Behavior Data")]
[PropertyOrder(3)]
[ShowInInspector]
[ReadOnly]
public class ModelBehaviorReadOnly : Attribute
{

}

#endregion

#region Settings Attribute
[IncludeMyAttributes]
[TitleGroup("Settings")]
[PropertyOrder(4)]
[ShowInInspector]
public class Settings : Attribute
{

}

[IncludeMyAttributes]
[ShowIf("showReadOnly")]
[TitleGroup("Settings")]
[PropertyOrder(4)]
[ShowInInspector]
[ReadOnly]
public class SettingsReadOnly : Attribute
{

}

#endregion











