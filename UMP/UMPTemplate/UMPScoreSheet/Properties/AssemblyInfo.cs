using System.Reflection;
using System.Runtime.InteropServices;
// 有关程序集的常规信息通过以下
// 特性集控制。更改这些特性值可修改
// 与程序集关联的信息。
using System.Windows;
using System.Windows.Markup;

[assembly: AssemblyTitle("UMPScoreSheet")]
[assembly: AssemblyDescription("VoiceCyber UMP ScoreSheet")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("VoiceCyber")]
[assembly: AssemblyProduct("UMPScoreSheet")]
[assembly: AssemblyCopyright("Copyright © 2016 VoiceCyber Technologies Ltd.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 将 ComVisible 设置为 false 使此程序集中的类型
// 对 COM 组件不可见。如果需要从 COM 访问此程序集中的类型，
// 则将该类型上的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("2489c5dc-5048-4ef4-a29f-304a93372d83")]

// 程序集的版本信息由下面四个值组成:
//
//      主版本
//      次版本 
//      生成号
//      修订号
//
// 可以指定所有这些值，也可以使用“生成号”和“修订号”的默认值，
// 方法是按如下所示使用“*”:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("8.3.2.1")]
[assembly: AssemblyFileVersion("8.03.002.001")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.SourceAssembly, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsPrefix("http://www.voicecyber.com/UMP/ScoreSheets", "vcsh")]
[assembly: XmlnsDefinition("http://www.voicecyber.com/UMP/ScoreSheets", "VoiceCyber.UMP.ScoreSheets")]
[assembly: XmlnsDefinition("http://www.voicecyber.com/UMP/ScoreSheets/Controls", "VoiceCyber.UMP.ScoreSheets.Controls")]
[assembly: XmlnsDefinition("http://www.voicecyber.com/UMP/ScoreSheets/Controls/Design", "VoiceCyber.UMP.ScoreSheets.Controls.Design")]
