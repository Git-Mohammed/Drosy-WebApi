﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Drosy.Domain.Shared.ErrorComponents.EFCore.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ErrorMessages_EFCore {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ErrorMessages_EFCore() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Drosy.Domain.Shared.ErrorComponents.EFCore.Resources.ErrorMessages.EFCore", typeof(ErrorMessages_EFCore).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to فشل حفظ التغييرات في قاعدة البيانات..
        /// </summary>
        public static string Error_EFCore_CanNotSaveChanges {
            get {
                return ResourceManager.GetString("Error_EFCore_CanNotSaveChanges", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to حدث تعارض في التزامن. تم تعديل البيانات التي تحاول حفظها بواسطة مستخدم آخر..
        /// </summary>
        public static string Error_EFCore_ConcurrencyConflict {
            get {
                return ResourceManager.GetString("Error_EFCore_ConcurrencyConflict", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  تم انتهاك أحد قيود قاعدة البيانات. يرجى التأكد من صحة البيانات والمحاولة مرة أخرى..
        /// </summary>
        public static string Error_EFCore_ConstraintViolation {
            get {
                return ResourceManager.GetString("Error_EFCore_ConstraintViolation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to فشل بدء أو إتمام عملية قاعدة البيانات..
        /// </summary>
        public static string Error_EFCore_FailedTransaction {
            get {
                return ResourceManager.GetString("Error_EFCore_FailedTransaction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to لم يتم إجراء أي تغيير على الخادم..
        /// </summary>
        public static string Error_EFCore_NoChanges {
            get {
                return ResourceManager.GetString("Error_EFCore_NoChanges", resourceCulture);
            }
        }
    }
}
