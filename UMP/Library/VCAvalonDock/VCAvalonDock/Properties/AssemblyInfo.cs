using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
// 有关程序集的常规信息通过以下
// 特性集控制。更改这些特性值可修改
// 与程序集关联的信息。
using System.Windows;
using System.Windows.Markup;

[assembly: AssemblyTitle("VCAvalonDock")]
[assembly: AssemblyDescription("VoiceCyber AvalonDock Control")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("VoiceCyber")]
[assembly: AssemblyProduct("VCAvalonDock")]
[assembly: AssemblyCopyright("Copyright © 2016 VoiceCyber Technologies Ltd.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 将 ComVisible 设置为 false 使此程序集中的类型
// 对 COM 组件不可见。如果需要从 COM 访问此程序集中的类型，
// 则将该类型上的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("762388e1-be74-4b16-a31c-a97226a5f472")]

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
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsPrefix("http://www.voicecyber.com/wpf/avalondock", "vcad")]
[assembly: XmlnsDefinition("http://www.voicecyber.com/wpf/avalondock", "VoiceCyber.Wpf.AvalonDock")]
[assembly: XmlnsDefinition("http://www.voicecyber.com/wpf/avalondock", "VoiceCyber.Wpf.AvalonDock.Controls")]
[assembly: XmlnsDefinition("http://www.voicecyber.com/wpf/avalondock", "VoiceCyber.Wpf.AvalonDock.Converters")]
[assembly: XmlnsDefinition("http://www.voicecyber.com/wpf/avalondock", "VoiceCyber.Wpf.AvalonDock.Layout")]
[assembly: XmlnsDefinition("http://www.voicecyber.com/wpf/avalondock", "VoiceCyber.Wpf.AvalonDock.Themes")]
