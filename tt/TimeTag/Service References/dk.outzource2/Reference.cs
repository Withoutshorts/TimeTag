﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TimeTag.dk.outzource2 {
    using System.Data;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="dk.outzource2.to_import_timetagSoap")]
    public interface to_import_timetagSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/timeout_importTimer_timetag", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string timeout_importTimer_timetag(System.Data.DataSet ds);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface to_import_timetagSoapChannel : TimeTag.dk.outzource2.to_import_timetagSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class to_import_timetagSoapClient : System.ServiceModel.ClientBase<TimeTag.dk.outzource2.to_import_timetagSoap>, TimeTag.dk.outzource2.to_import_timetagSoap {
        
        public to_import_timetagSoapClient() {
        }
        
        public to_import_timetagSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public to_import_timetagSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public to_import_timetagSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public to_import_timetagSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string timeout_importTimer_timetag(System.Data.DataSet ds) {
            return base.Channel.timeout_importTimer_timetag(ds);
        }
    }
}
