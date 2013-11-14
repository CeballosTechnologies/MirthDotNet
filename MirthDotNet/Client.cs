﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MirthDotNet.Model;
using MirthDotNet.Plugins;

namespace MirthDotNet
{
    /// <remarks>
    /// This code is ported from: https://svn.mirthcorp.com/connect/trunk/server/src/com/mirth/connect/client/core/Client.java
    /// </remarks>
    public class Client
    {
        public const string USER_SERVLET = "/users";
        public const string CHANNEL_SERVLET = "/channels";
        public const string CONFIGURATION_SERVLET = "/configuration";
        public const string CHANNEL_STATUS_SERVLET = "/channelstatus";
        public const string CHANNEL_STATISTICS_SERVLET = "/channelstatistics";
        public const string MESSAGE_SERVLET = "/messages";
        public const string EVENT_SERVLET = "/events";
        public const string ALERT_SERVLET = "/alerts";
        public const string TEMPLATE_SERVLET = "/codetemplates";
        public const string EXTENSION_SERVLET = "/extensions";
        public const string ENGINE_SERVLET = "/engine";

        public Client(string address, int timeout = ServerConnection.DefaultTimeout)
        {
            this.address = address;
            this.connection = new ServerConnection(address, timeout);
            this.ServerLog = new ServerLog(this);
            this.DashboardConnectorStatus = new DashboardConnectorStatus(this);
        }

        private readonly string address;
        private readonly ServerConnection connection;

        private XmlSerializer GetSerializer<T>()
        {
            return new XmlSerializer(typeof(T));
        }

        private T ToObject<T>(string result)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(result)))
            {
                return (T)GetSerializer<T>().Deserialize(stream);
            }
        }

        public LoginStatus Login(string username, string password, string version)
        {
            var data = new Parameters();
            data.Add("op", Operations.USER_LOGIN.Name);
            data.Add("username", username);
            data.Add("password", password);
            data.Add("version", version);
            return ToObject<LoginStatus>(connection.ExecutPostMethod(USER_SERVLET, data));
        }

        public void Logout()
        {
            var data = new Parameters();
            data.Add("op", Operations.USER_LOGOUT.Name);
            connection.ExecutPostMethod(USER_SERVLET, data);
        }

        public string GetServerId()
        {
            var data = new Parameters();
            data.Add("op", Operations.CONFIGURATION_SERVER_ID_GET.Name);
            return connection.ExecutPostMethod(CONFIGURATION_SERVLET, data);
        }

        public string GetServerTimezone()
        {
            var data = new Parameters();
            data.Add("op", Operations.CONFIGURATION_SERVER_TIMEZONE_GET.Name);
            return connection.ExecutPostMethod(CONFIGURATION_SERVLET, data);
        }

        public string GetStatus()
        {
            var data = new Parameters();
            data.Add("op", Operations.CONFIGURATION_STATUS_GET.Name);
            return connection.ExecutPostMethod(CONFIGURATION_SERVLET, data);
        }

        public string GetBuildDate()
        {
            var data = new Parameters();
            data.Add("op", Operations.CONFIGURATION_BUILD_DATE_GET.Name);
            return connection.ExecutPostMethod(CONFIGURATION_SERVLET, data);
        }

        public string GetVersion()
        {
            var data = new Parameters();
            data.Add("op", Operations.CONFIGURATION_VERSION_GET.Name);
            return connection.ExecutPostMethod(CONFIGURATION_SERVLET, data);
        }

        public DashboardStatusList GetChannelStatusList()
        {
            var data = new Parameters();
            data.Add("op", Operations.CHANNEL_GET_STATUS.Name);
            var r = connection.ExecutPostMethod(CHANNEL_STATUS_SERVLET, data);
            return ToObject<DashboardStatusList>(r);
        }

        public ChannelStatistics GetStatistics(string channelId)
        {
            var data = new Parameters();
            data.Add("op", Operations.CHANNEL_STATS_GET.Name);
            data.Add("id", channelId);
            var r = connection.ExecutPostMethod(CHANNEL_STATISTICS_SERVLET, data);
            return ToObject<ChannelStatistics>(r);
        }

        public ServerSettings GetServerSettings()
        {
            var data = new Parameters();
            data.Add("op", Operations.CONFIGURATION_SERVER_SETTINGS_GET.Name);
            var r = connection.ExecutPostMethod(CONFIGURATION_SERVLET, data);
            return ToObject<ServerSettings>(r);
        }

        public string GetCodeTemplate()
        {
            var data = new Parameters();
            data.Add("op", Operations.CODE_TEMPLATE_GET.Name);
            data.Add("codeTemplate", "");
            var r = connection.ExecutPostMethod(TEMPLATE_SERVLET, data);
            return r;
        }

        public string InvokePluginMethod(string pluginName, string method, string parameters)
        {
            var data = new Parameters();
            data.Add("op", Operations.PLUGIN_SERVICE_INVOKE.Name);
            data.Add("name", pluginName);
            data.Add("method", method);
            if (parameters == null)
            {
                data.Add("object", "<null/>");
            }
            else
            {
                data.Add("object", parameters);
            }
            var r = connection.ExecutPostMethod(EXTENSION_SERVLET, data);
            return r;
        }

        public T InvokePluginMethod<T>(string pluginName, string method, string parameters)
        {
            return ToObject<T>(InvokePluginMethod(pluginName, method, parameters));
        }

        public ServerLog ServerLog { get; private set; }
        public DashboardConnectorStatus DashboardConnectorStatus { get; private set; }
    }
}
