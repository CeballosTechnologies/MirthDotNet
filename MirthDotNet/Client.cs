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

        private string FromObject<T>(object instance)
        {
            using (var stream = new MemoryStream())
            {
                GetSerializer<T>().Serialize(stream, instance);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// Authenticate and login as a user for this instance of the Client.
        /// </summary>
        public LoginStatus Login(string username, string password, string version)
        {
            var data = new Parameters();
            data.Add("op", Operations.USER_LOGIN.Name);
            data.Add("username", username);
            data.Add("password", password);
            data.Add("version", version);
            return ToObject<LoginStatus>(connection.ExecutPostMethod(USER_SERVLET, data));
        }

        /// <summary>
        /// Logs out the current user for this instance of the Client.
        /// </summary>
        public void Logout()
        {
            var data = new Parameters();
            data.Add("op", Operations.USER_LOGOUT.Name);
            connection.ExecutPostMethod(USER_SERVLET, data);
        }

        /// <summary>
        /// Gets the unique Server ID for the Mirth Server.
        /// </summary>
        public string GetServerId()
        {
            var data = new Parameters();
            data.Add("op", Operations.CONFIGURATION_SERVER_ID_GET.Name);
            return connection.ExecutPostMethod(CONFIGURATION_SERVLET, data);
        }

        /// <summary>
        /// Gets the configured TimeZone of the Mirth Server.
        /// </summary>
        public string GetServerTimezone()
        {
            var data = new Parameters();
            data.Add("op", Operations.CONFIGURATION_SERVER_TIMEZONE_GET.Name);
            return connection.ExecutPostMethod(CONFIGURATION_SERVLET, data);
        }

        /// <summary>
        /// Gets the current Status code for the Mirth Server.
        /// </summary>
        public string GetStatus()
        {
            var data = new Parameters();
            data.Add("op", Operations.CONFIGURATION_STATUS_GET.Name);
            return connection.ExecutPostMethod(CONFIGURATION_SERVLET, data);
        }

        /// <summary>
        /// Gets the Build Date for the Mirth Server.
        /// </summary>
        public string GetBuildDate()
        {
            var data = new Parameters();
            data.Add("op", Operations.CONFIGURATION_BUILD_DATE_GET.Name);
            return connection.ExecutPostMethod(CONFIGURATION_SERVLET, data);
        }

        /// <summary>
        /// Gets the version number of the Mirth Server.
        /// </summary>
        public string GetVersion()
        {
            var data = new Parameters();
            data.Add("op", Operations.CONFIGURATION_VERSION_GET.Name);
            return connection.ExecutPostMethod(CONFIGURATION_SERVLET, data);
        }

        /// <summary>
        /// Gets the DashboardStatus List of all deployed channels.
        /// </summary>
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
            data.Add("codeTemplate", "<null/>");
            var r = connection.ExecutPostMethod(TEMPLATE_SERVLET, data);
            return r;
        }

        /// <summary>
        /// Gets a set of all tag names currently used by a channel.
        /// </summary>
        public HashSet<string> GetChannelTags()
        {
            var data = new Parameters();
            data.Add("op", Operations.CHANNEL_GET_TAGS.Name);
            var r = connection.ExecutPostMethod(CHANNEL_SERVLET, data);
            return new HashSet<string>(ToObject<MirthLinkedHashSet>(r).Items);
        }

        //        Executing POST [getConnectorNames] On [/channels] With [op=getConnectorNames, channelId=1f76977d-46c5-41c3-8c84-ec683f02af89]
        //Executing POST [getMetaDataColumns] On [/channels] With [op=getMetaDataColumns, channelId=1f76977d-46c5-41c3-8c84-ec683f02af89]
        //Executing POST [getMaxMessageId] On [/messages] With [op=getMaxMessageId, channelId=1f76977d-46c5-41c3-8c84-ec683f02af89]

        /// <summary>
        /// Returns a list of all connectors for the given channel.
        /// </summary>
        public MirthLinkedHashMap<MirthIntLinkedHashMapItem> GetConnectorNames(string channelId)
        {
            var data = new Parameters();
            data.Add("op", Operations.CHANNEL_GET_CONNECTOR_NAMES.Name);
            data.Add("channelId", channelId);
            var r = connection.ExecutPostMethod(CHANNEL_SERVLET, data);
            return ToObject<MirthLinkedHashMap<MirthIntLinkedHashMapItem>>(r);
        }

        /// <summary>
        /// Returns a list of custom metadata columns for the given channel.
        /// </summary>
        public MetaDataColumnList GetMetaDataColumns(string channelId)
        {
            var data = new Parameters();
            data.Add("op", Operations.CHANNEL_GET_METADATA_COLUMNS.Name);
            data.Add("channelId", channelId);
            var r = connection.ExecutPostMethod(CHANNEL_SERVLET, data);
            return ToObject<MetaDataColumnList>(r);
        }

        /// <summary>
        /// Returns the maximum message id for the given channel
        /// </summary>
        public long GetMaxMessageId(string channelId)
        {
            var data = new Parameters();
            data.Add("op", Operations.MESSAGE_GET_MAX_ID.Name);
            data.Add("channelId", channelId);
            var r = connection.ExecutPostMethod(MESSAGE_SERVLET, data);
            return long.Parse(r);
        }

        /// <summary>
        /// Returns a list of messages for a given channel matching a filter
        /// </summary>
        public MessageList GetMessages(string channelId, MessageFilter filter, bool includeContent = false, int offset = 0, int limit = 51)
        {
            var data = new Parameters();
            data.Add("op", Operations.MESSAGE_GET.Name);
            data.Add("channelId", channelId);
            data.Add("filter", FromObject<MessageFilter>(filter));
            data.Add("includeContent", (includeContent) ? "y" : "n");
            data.Add("offset", offset.ToString());
            data.Add("limit", limit.ToString());
            var r = connection.ExecutPostMethod(MESSAGE_SERVLET, data);
            return ToObject<MessageList>(r);
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
