using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HttpListnerEx
{
    public class Worker
    {
        private HttpListenerContext context;
 
       public Worker(HttpListenerContext context)
       {
          this.context = context;
       }
 
       public void ProcessRequest()
       {
          string msg = context.Request.HttpMethod + " " + context.Request.Url;
          Console.WriteLine(msg);
 
          StringBuilder sb = new StringBuilder();
          sb.Append("<html><body><h1>" + msg + "</h1>");
          DumpRequest(context.Request, sb);
          sb.Append("</body></html>");
 
          byte[] b = Encoding.UTF8.GetBytes(sb.ToString());
          context.Response.ContentLength64 = b.Length;
          context.Response.OutputStream.Write(b, 0, b.Length);
          context.Response.OutputStream.Close();
       }
 
       private void DumpRequest(HttpListenerRequest request, StringBuilder sb)
       {
          DumpObject(request, sb);
      }
 
       private void DumpObject(object o, StringBuilder sb)
       {
          DumpObject(o, sb, true);
       }
 
       private void DumpObject(object o, StringBuilder sb, bool ulli)
       {
          if (ulli)
             sb.Append("<ul>");
 
          if (o is string || o is int || o is long || o is double)
          {
             if(ulli)
                sb.Append("<li>");
 
             sb.Append(o.ToString());
 
             if(ulli)
                sb.Append("</li>");
          }
          else
          {
             Type t = o.GetType();
             foreach (PropertyInfo p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
             {
                sb.Append("<li><b>" + p.Name + ":</b> ");
                object val = null;
 
                try
                {
                   val = p.GetValue(o, null);
                }
                catch {}

                if (val is string || val is int || val is long || val is double)
                   sb.Append(val);
                else
 
                if (val != null)
                {
                   Array arr = val as Array;
                   if (arr == null)
                   {
                      NameValueCollection nv = val as NameValueCollection;
                      if (nv == null)
                      {
                         IEnumerable ie = val as IEnumerable;
                         if (ie == null)
                            sb.Append(val.ToString());
                         else
                            foreach (object oo in ie)
                               DumpObject(oo, sb);
                      }
                      else
                      {
                         sb.Append("<ul>");
                         foreach (string key in nv.AllKeys)
                         {
                            sb.AppendFormat("<li>{0} = ", key);
                            DumpObject(nv[key], sb,false);
                            sb.Append("</li>");
                         }
                         sb.Append("</ul>");
                      }
                   }
                   else
                      foreach (object oo in arr)
                         DumpObject(oo, sb);
                }
                else
                {
                   sb.Append("<i>null</i>");
                }
                sb.Append("</li>");
             }
          }
          if (ulli)
             sb.Append("</ul>");
       }
    }
}
