﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RMSAutoAPI.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RMSAutoAPI.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Доступ запрещен.
        /// </summary>
        internal static string ErrorAccessDenied {
            get {
                return ResourceManager.GetString("ErrorAccessDenied", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Превышен лимит запросов.
        /// </summary>
        internal static string ErrorMaxRequests {
            get {
                return ResourceManager.GetString("ErrorMaxRequests", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error Not Found.
        /// </summary>
        internal static string ErrorNotFound {
            get {
                return ResourceManager.GetString("ErrorNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string GroupDescription {
            get {
                return ResourceManager.GetString("GroupDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to API.RMSAUTO представляет собой REST-сервис.  Для обращения к методам api необходимо пройти авторизацию на сервисе посредством персонального логина/пароля. В ответ будет сгенерирован Bearer токен, который необходимо передавать в HTTP заголовок вызываемого метода..
        /// </summary>
        internal static string HeaderDescription {
            get {
                return ResourceManager.GetString("HeaderDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to API.RMSAUTO.
        /// </summary>
        internal static string HeaderTitle {
            get {
                return ResourceManager.GetString("HeaderTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to v1.
        /// </summary>
        internal static string HeaderVersion {
            get {
                return ResourceManager.GetString("HeaderVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to brand.
        /// </summary>
        internal static string LogTypeBrand {
            get {
                return ResourceManager.GetString("LogTypeBrand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to pn.
        /// </summary>
        internal static string LogTypePartNumber {
            get {
                return ResourceManager.GetString("LogTypePartNumber", resourceCulture);
            }
        }
    }
}
