using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml;

namespace MVC_32_State_Managament_Uygulama
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start() // Uygulamaya start denildiği anda tetiklenir. Projenin çalışma anında yapılması gerekenleri barındırır.
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            Application["onlineUser"] = 0;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // Eğer Session_Start event'i tetiklenmiş ise, kullanıcı siteye bir istekte bulunmuş demektir. Bu istek durumunda, online kullanıcı sayısı veya diğer kontrol ettiğimiz olayları değiştirebilirsiniz.
            Application.Lock();
            Application["onlineUser"] = ((int)Application["onlineUser"]) + 1;
            Application.UnLock();
        }

        // Uygulamadaki herhangi bir dosyaya talep(request) geldiğinde tetiklenir.
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        // FormsAuthentication kullanıldığında kullanıcının sisteme başarılı şekilde giriş yapması durumunda tetiklenecek olan olaydır.
        protected void Application_AuthenticationRequest(object sender, EventArgs e)
        {
        }

        // Uygulamanın herhangi bir noktasında hata meydana geldiği zaman hatanın işlenmesi için tetiklenen olaydır.
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception sonHata = Server.GetLastError();
            // Aldığımız son hata içerisinde bir detay mesajı varsa o mesajı, yoksa genel hata mesajını yazdıralım.
            string hataMesaji = sonHata.InnerException != null ? sonHata.InnerException.Message : sonHata.Message;
            DateTime hataZamani = DateTime.Now;
            string kullaniciIP = Request.ServerVariables["REMOTE_ADDR"];

            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("../HataLog.xml"));

            XmlElement eleman = doc.CreateElement("Hata");

            XmlNode node_Mesaj = doc.CreateNode(XmlNodeType.Element, "HataMesaji", null);
            node_Mesaj.InnerText = hataMesaji;

            XmlNode node_Tarih = doc.CreateNode(XmlNodeType.Element, "HataTarihi", null);
            node_Tarih.InnerText = hataZamani.ToString();

            XmlNode node_IP = doc.CreateNode(XmlNodeType.Element, "MakineIP", null);
            node_IP.InnerText = kullaniciIP;

            eleman.AppendChild(node_Mesaj);
            eleman.AppendChild(node_Tarih);
            eleman.AppendChild(node_IP);
            doc.DocumentElement.AppendChild(eleman);
            doc.Save(Server.MapPath("../HataLog.xml"));
        }

        // Eğer Session_End eventi tetiklenmiş ise, bir kullanıcının oturumunun sunucu tarafında sonlandığı anlamına gelir. Doğal olarak o kullanıcıya ait veriler silineceği için arttırılan online sayısı azaltılabilir.
        protected void Session_End(object sender, EventArgs e)
        {
            Application.Lock();
            Application["onlineUser"] = ((int)Application["onlineUser"]) - 1;
            Application.UnLock();
        }

        // Uygulama kapandığında tetiklenen olaydır.
        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}
// Application(Uygulama): Session gibi içerisinde veri tutmak için vardır. Sunucu taraflı oluşur. Session'dan farkı şudur:
// Session her kullanıcıya özel olarak yeni oluşturulurken Application nesnesi tek bir tane olup her kullanıcı için ortaktır.
// Yani her kullanıcı aynı nesneye erişim sağlar.