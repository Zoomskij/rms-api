using RMSAutoAPI.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace RMSAutoAPI.Models.Orders
{
    public class ServiceProxy
    {
        private ex_rmsauto_storeEntities _db = new ex_rmsauto_storeEntities();
        private static readonly Dictionary<Type, ProxyType> _proxyTypeCache = new Dictionary<Type, ProxyType>();
        private static readonly object _sync = new object();
        private ProxyType _proxyType;

       // public static readonly ServiceProxy Default = new ServiceProxy();

        public ServiceProxy(ex_rmsauto_storeEntities db)
        {
            _db = db;
            Type type = this.GetType();
            if (!_proxyTypeCache.ContainsKey(type))
                lock (_sync)
                    if (!_proxyTypeCache.ContainsKey(type))
                        _proxyTypeCache.Add(type, new ProxyType(type));
            _proxyType = _proxyTypeCache[type];
        }

        public ServiceProxy(string url, string username, string password, int? timeout)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("Acctg_service_url cannot be empty", "url");
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Acctg_service_username cannot be empty", "username");




            Type type = this.GetType();
            if (!_proxyTypeCache.ContainsKey(type))
                lock (_sync)
                    if (!_proxyTypeCache.ContainsKey(type))
                        _proxyTypeCache.Add(type, new ProxyType(type));
            _proxyType = _proxyTypeCache[type];
        }

        /// <summary>
        /// Новый метод отправки заказа (без использования сервиса, просто сохраняем этот же xml-ник в БД)
        /// </summary>
        [ServiceMethod("SendOrder", typeof(SendOrderEnvelope), typeof(SendOrderResultsEnvelope))]
        public void SendOrder(OrderInfo order)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            SaveRequestToDB("order", "SendOrder", new SendOrderEnvelope { ClientId = order.ClientId, Order = order });
        }
        private void SaveRequestToDB(string requestType, string methodName, Envelope argsEnvelope)
        {
            Type type = this.GetType();
            _proxyType = new ProxyType(type);
            var method = _proxyType.GetMethod(methodName);
            var messageSerializer = new MessageSerializer(method);

            // получаем xml-запроса как и раньше
            string requestXml = messageSerializer.SerializeRequest("", "", argsEnvelope);
            // убираем xml-заголовок (<?xml version="1.0" encoding="utf-16"?>), чтобы сохранить в БД
            requestXml = Regex.Replace(requestXml, @"<\?xml.*\?>", string.Empty);

            // теперь просто сохраняем его в БД
            //using (var dc = new DCWrappersFactory<StoreDataContext>())
            //{
            try
            {
                _db.Database.ExecuteSqlCommand("INSERT into Acctg.Requests VALUES ({0}, {1}, GETDATE())", requestType, requestXml);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                //Теперь закрываемся в фабрике
                //
            }
            //}
        }
    }
    }