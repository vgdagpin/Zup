﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Zup.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.10.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int FormLocationX {
            get {
                return ((int)(this["FormLocationX"]));
            }
            set {
                this["FormLocationX"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int FormLocationY {
            get {
                return ((int)(this["FormLocationY"]));
            }
            set {
                this["FormLocationY"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TimesheetsFolder {
            get {
                return ((string)(this["TimesheetsFolder"]));
            }
            set {
                this["TimesheetsFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public int ItemsToShow {
            get {
                return ((int)(this["ItemsToShow"]));
            }
            set {
                this["ItemsToShow"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AutoOpenUpdateWindow {
            get {
                return ((bool)(this["AutoOpenUpdateWindow"]));
            }
            set {
                this["AutoOpenUpdateWindow"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.9")]
        public double EntryListOpacity {
            get {
                return ((double)(this["EntryListOpacity"]));
            }
            set {
                this["EntryListOpacity"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DbPath {
            get {
                return ((string)(this["DbPath"]));
            }
            set {
                this["DbPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("30")]
        public int TrimDaysToKeep {
            get {
                return ((int)(this["TrimDaysToKeep"]));
            }
            set {
                this["TrimDaysToKeep"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("~StartedOnTicks~^~Task~^~Comments~^~TaskCode~^~Duration~^False^False")]
        public string ExportRowFormat {
            get {
                return ((string)(this["ExportRowFormat"]));
            }
            set {
                this["ExportRowFormat"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".fd")]
        public string ExportFileExtension {
            get {
                return ((string)(this["ExportFileExtension"]));
            }
            set {
                this["ExportFileExtension"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("14")]
        public int NumDaysOfDataToLoad {
            get {
                return ((int)(this["NumDaysOfDataToLoad"]));
            }
            set {
                this["NumDaysOfDataToLoad"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ShowQueuedTasks {
            get {
                return ((bool)(this["ShowQueuedTasks"]));
            }
            set {
                this["ShowQueuedTasks"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ShowRankedTasks {
            get {
                return ((bool)(this["ShowRankedTasks"]));
            }
            set {
                this["ShowRankedTasks"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ShowClosedTasks {
            get {
                return ((bool)(this["ShowClosedTasks"]));
            }
            set {
                this["ShowClosedTasks"] = value;
            }
        }
    }
}
