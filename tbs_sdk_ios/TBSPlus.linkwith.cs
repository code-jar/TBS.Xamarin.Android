using System;
using ObjCRuntime;

[assembly: LinkWith ("TBSPlus.a", LinkTarget.ArmV7 | LinkTarget.ArmV7s | LinkTarget.Simulator, ForceLoad = true)]
