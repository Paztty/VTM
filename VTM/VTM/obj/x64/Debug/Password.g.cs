﻿#pragma checksum "..\..\..\Password.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "741D7681D97E2559C59CBF17F67291BFE88B120A52B7CE55BA41A4A5D3D5362B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using VTM;


namespace VTM {
    
    
    /// <summary>
    /// Password
    /// </summary>
    public partial class Password : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 39 "..\..\..\Password.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ClosePassword;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\..\Password.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox AdminPassword;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\Password.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btOK;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\..\Password.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btCancel;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\..\Password.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock LoginStatus;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/VTM;component/password.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Password.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\..\Password.xaml"
            ((VTM.Password)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ClosePassword = ((System.Windows.Controls.Button)(target));
            
            #line 39 "..\..\..\Password.xaml"
            this.ClosePassword.Click += new System.Windows.RoutedEventHandler(this.ClosePassword_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.AdminPassword = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 4:
            this.btOK = ((System.Windows.Controls.Button)(target));
            
            #line 66 "..\..\..\Password.xaml"
            this.btOK.Click += new System.Windows.RoutedEventHandler(this.btOK_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btCancel = ((System.Windows.Controls.Button)(target));
            
            #line 74 "..\..\..\Password.xaml"
            this.btCancel.Click += new System.Windows.RoutedEventHandler(this.btCancel_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.LoginStatus = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

