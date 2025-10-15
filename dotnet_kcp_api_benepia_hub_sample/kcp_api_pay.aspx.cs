using System;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.ConstrainedExecution;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Engines;

namespace Payment
{
    public partial class Kcp_api_pay : System.Web.UI.Page
    {
        // 인증서 정보(직렬화)
        //public string KCP_CERT_INFO = "-----BEGIN CERTIFICATE-----MIIDgTCCAmmgAwIBAgIHBy4lYNG7ojANBgkqhkiG9w0BAQsFADBzMQswCQYDVQQGEwJLUjEOMAwGA1UECAwFU2VvdWwxEDAOBgNVBAcMB0d1cm8tZ3UxFTATBgNVBAoMDE5ITktDUCBDb3JwLjETMBEGA1UECwwKSVQgQ2VudGVyLjEWMBQGA1UEAwwNc3BsLmtjcC5jby5rcjAeFw0yMTA2MjkwMDM0MzdaFw0yNjA2MjgwMDM0MzdaMHAxCzAJBgNVBAYTAktSMQ4wDAYDVQQIDAVTZW91bDEQMA4GA1UEBwwHR3Vyby1ndTERMA8GA1UECgwITG9jYWxXZWIxETAPBgNVBAsMCERFVlBHV0VCMRkwFwYDVQQDDBAyMDIxMDYyOTEwMDAwMDI0MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAppkVQkU4SwNTYbIUaNDVhu2w1uvG4qip0U7h9n90cLfKymIRKDiebLhLIVFctuhTmgY7tkE7yQTNkD+jXHYufQ/qj06ukwf1BtqUVru9mqa7ysU298B6l9v0Fv8h3ztTYvfHEBmpB6AoZDBChMEua7Or/L3C2vYtU/6lWLjBT1xwXVLvNN/7XpQokuWq0rnjSRThcXrDpWMbqYYUt/CL7YHosfBazAXLoN5JvTd1O9C3FPxLxwcIAI9H8SbWIQKhap7JeA/IUP1Vk4K/o3Yiytl6Aqh3U1egHfEdWNqwpaiHPuM/jsDkVzuS9FV4RCdcBEsRPnAWHz10w8CX7e7zdwIDAQABox0wGzAOBgNVHQ8BAf8EBAMCB4AwCQYDVR0TBAIwADANBgkqhkiG9w0BAQsFAAOCAQEAg9lYy+dM/8Dnz4COc+XIjEwr4FeC9ExnWaaxH6GlWjJbB94O2L26arrjT2hGl9jUzwd+BdvTGdNCpEjOz3KEq8yJhcu5mFxMskLnHNo1lg5qtydIID6eSgew3vm6d7b3O6pYd+NHdHQsuMw5S5z1m+0TbBQkb6A9RKE1md5/Yw+NymDy+c4NaKsbxepw+HtSOnma/R7TErQ/8qVioIthEpwbqyjgIoGzgOdEFsF9mfkt/5k6rR0WX8xzcro5XSB3T+oecMS54j0+nHyoS96/llRLqFDBUfWn5Cay7pJNWXCnw4jIiBsTBa3q95RVRyMEcDgPwugMXPXGBwNoMOOpuQ==-----END CERTIFICATE-----";

        // 인증서 정보(직렬화) / AGL 리얼 인증키
        public string KCP_CERT_INFO = "-----BEGIN CERTIFICATE-----MIIDjDCCAnSgAwIBAgIHBzHPFJD/mDANBgkqhkiG9w0BAQsFADBzMQswCQYDVQQGEwJLUjEOMAwGA1UECAwFU2VvdWwxEDAOBgNVBAcMB0d1cm8tZ3UxFTATBgNVBAoMDE5ITktDUCBDb3JwLjETMBEGA1UECwwKSVQgQ2VudGVyLjEWMBQGA1UEAwwNc3BsLmtjcC5jby5rcjAeFw0yNTA5MDMwNDM4MTRaFw0zMDA5MDIwNDM4MTRaMHsxCzAJBgNVBAYTAktSMQ4wDAYDVQQIDAVTZW91bDEQMA4GA1UEBwwHR3Vyby1ndTEWMBQGA1UECgwNTkhOIEtDUCBDb3JwLjEXMBUGA1UECwwOUEdXRUJERVYgVGVhbS4xGTAXBgNVBAMMEDIwMjUwOTAzMTAwMTA1OTUwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCNaqGX5L3bnKJhjBD3QJo9hsAz7PF7dIwudvnx2Wy3ln2+9vTmpkIRVZoFye4tNTmoEHzoBcdNARGNC4/LM1trjRBYGFhnHhXtn48ue7B2zuhCHlX0CTDLOPnIU7ZhKvrgiq7f2ABKm77hQjmLFQ+BCJAAI/sPbwG+4U7MQRXlKlxW7+sxb8KHr9hT83WnVBQXHqyQmtmaiBSIitdge/8YFk1+zOjTh4p5ODs2tJwzfLUM/kc+HMLVrFqGrHzgGz+VR3o9uFC7VwJoaRMIdCF7Ig6OcqC+cdCHpPX8PaWguEQOKTILcDB1CB5yF6VNQbyFeDdbqjHrute1KNiVIiC9AgMBAAGjHTAbMA4GA1UdDwEB/wQEAwIHgDAJBgNVHRMEAjAAMA0GCSqGSIb3DQEBCwUAA4IBAQCIEk9/kN/oGFditHBtErpFqrqFl33FNTO+DH6QjcJJEVropNjq07YtRBaNOYY6na7kiXK+YsAV8Omk9VIg/xX91mwFxv+qVCBtzVLAtuROrt7CR74ZiLE1LEcbkhC/LBsjJzJTfb96wurI2GVTO9RAlfRZYQoju2/xBl52um0L1/CVElYp/I3hGYLiEf/1NPyH/18M8g+lYFuLroraLD/NK66d+M4vtYibpvouUSe9lbGGREoLZbPxdrPkTO2aHSskLP0EjbFZo55a0Yau+c0FysqF5zRcmhArp4RGM+8nIbYhT9tyVmKTgUr/8yumcS/5iG5SIqMZS8TWwWy5lPsP-----END CERTIFICATE-----";

        // 요청 전문
        protected string req_tx;
        protected string site_cd;
        protected string req_param;

        protected string pay_method; // 결제수단
        protected string amount;  // 총결제금액    
        protected string currency; // 화폐단위
        protected string cust_ip;  // 결제 고객 ip
        protected string pt_mny;  // 포인트 결제 금액

        protected string pt_issue;  // 포인트기관
        protected string pt_txtype;  // 포인트전문유형    
        protected string pt_idno;  // 포인트계정 아이디
        protected string pt_pwd;  // 포인트계정 비밀번호
        protected string pt_memcorp_cd;  // 기관할당 코드
        protected string pt_paycode; // 결제코드

        protected string ordr_idxx;  // 주문번호
        protected string good_name;  // 상품명    
        protected string buyr_name;  // 구매자명
        protected string buyr_mail;  // 구매자 이메일
        protected string buyr_tel2;  // 구매자 휴대폰번호

        //취소 요청 정보
        protected string kcp_sign_data;  // 변경유형
        protected string mod_type;  // 취소사유    
        protected string mod_desc;  // 재승인 요청금액
        protected string mod_mny;  // 재승인시 상품명
        protected string mod_ordr_goods;  // 재승인시 상품명
        protected string mod_ordr_idxx; //  재승인시 주문번호
        protected string tno;  // 거래번호

        protected string target_URL; // 요청 API

        // 응답 정보
        protected string res_cd;
        protected string res_msg;
        protected string res_en_msg;
        protected string res_param;
        protected string rsv_pnt;

        protected string pnt_amount;
        protected string pnt_app_no;
        protected string pnt_app_time;


        protected void Page_Load(object sender, EventArgs e)
        {
            req_tx = Request.Form["req_tx"]; // 기능 별 구분 값

            if (req_tx == "query") //조회
            {
                /* =  조회 API URL ============================================================= */
                //target_URL = "https://stg-spl.kcp.co.kr/gw/hub/v1/payment"; // 개발서버
                target_URL = "https://spl.kcp.co.kr/gw/hub/v1/payment"; // 운영서버
                // 요청정보
                site_cd = Request.Form["site_cd"];
                pay_method = Request.Form["pay_method"];
                amount = "10";
                currency = "410";
                cust_ip = Request.Form["cust_ip"];
                pt_issue = Request.Form["pt_issue"];
                pt_txtype = "97000000";
                pt_idno = Request.Form["pt_idno"];
                pt_pwd = Request.Form["pt_pwd"];
                //pt_memcorp_cd = Request.Form["pt_memcorp_cd"];
                pt_memcorp_cd = "5555"; // 
                ordr_idxx = Request.Form["ordr_idxx"];

                req_param = "{\"site_cd\" : \"" + site_cd + "\"," +
                                  "\"kcp_cert_info\":\"" + KCP_CERT_INFO + "\"," +
                                  "\"pay_method\":\"" + pay_method + "\"," +
                                  "\"amount\":\"" + amount + "\"," +
                                  "\"currency\":\"" + currency + "\"," +
                                  "\"cust_ip\":\"" + cust_ip + "\"," +
                                  "\"pt_issue\":\"" + pt_issue + "\"," +
                                  "\"pt_txtype\":\"" + pt_txtype + "\"," +
                                  "\"pt_idno\":\"" + pt_idno + "\"," +
                                  "\"pt_pwd\":\"" + pt_pwd + "\"," +
                                  "\"pt_memcorp_cd\":\"" + pt_memcorp_cd + "\"," +
                                  "\"ordr_idxx\":\"" + ordr_idxx + "\"}";

                // SSL/ TLS 보안 채널을 만들수 없습니다. 오류 발생 시 추가 (프레임워크 버전을 올렸음에도 안될 경우 추가)
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                // API REQ
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(target_URL);
                req.Method = "POST";
                req.ContentType = "application/json";

                byte[] byte_req = Encoding.UTF8.GetBytes(req_param);
                req.ContentLength = byte_req.Length;

                Stream st = req.GetRequestStream();
                st.Write(byte_req, 0, byte_req.Length);
                st.Close();

                // API RES
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader st_read = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                res_param = st_read.ReadToEnd();

                st_read.Close();
                res.Close();

                // RES JSON DATA Parsing
                JObject json_data = JObject.Parse(res_param);
                res_cd = json_data["res_cd"].ToString();
                res_msg = json_data["res_msg"].ToString();

                if (res_cd == "0000")
                {
                    rsv_pnt = json_data["rsv_pnt"].ToString();
                }
            }

            if (req_tx == "pay") // 결제
            {
                /* =  결제 API URL ============================================================= */
                //target_URL = "https://stg-spl.kcp.co.kr/gw/hub/v1/payment"; // 개발서버
                target_URL = "https://spl.kcp.co.kr/gw/hub/v1/payment"; // 운영서버
                // 요청정보
                site_cd = Request.Form["site_cd"];
                pay_method = Request.Form["pay_method"];
                amount = Request.Form["amount"];
                currency = "410";
                cust_ip = Request.Form["cust_ip"];
                pt_issue = Request.Form["pt_issue"];
                pt_txtype = "91200000";
                pt_idno = Request.Form["pt_idno"];
                pt_pwd = Request.Form["pt_pwd"];
                pt_memcorp_cd = Request.Form["pt_memcorp_cd"];
                pt_paycode = "04";
                pt_mny = Request.Form["amount"];
                ordr_idxx = Request.Form["ordr_idxx"];
                good_name = Request.Form["good_name"];
                buyr_name = Request.Form["buyr_name"];
                buyr_mail = Request.Form["buyr_mail"];
                buyr_tel2 = Request.Form["buyr_tel2"];

                req_param = "{\"site_cd\" : \"" + site_cd + "\"," +
                                  "\"kcp_cert_info\":\"" + KCP_CERT_INFO + "\"," +
                                  "\"pay_method\":\"" + pay_method + "\"," +
                                  "\"amount\":\"" + amount + "\"," +
                                  "\"currency\":\"" + currency + "\"," +
                                  "\"cust_ip\":\"" + cust_ip + "\"," +
                                  "\"pt_issue\":\"" + pt_issue + "\"," +
                                  "\"pt_txtype\":\"" + pt_txtype + "\"," +
                                  "\"pt_idno\":\"" + pt_idno + "\"," +
                                  "\"pt_pwd\":\"" + pt_pwd + "\"," +
                                  "\"pt_memcorp_cd\":\"" + pt_memcorp_cd + "\"," +
                                  "\"pt_paycode\":\"" + pt_paycode + "\"," +
                                  "\"pt_mny\":\"" + pt_mny + "\"," +
                                  "\"ordr_idxx\":\"" + ordr_idxx + "\"," +
                                  "\"good_name\":\"" + good_name + "\"," +
                                  "\"buyr_name\":\"" + buyr_name + "\"," +
                                  "\"buyr_mail\":\"" + buyr_mail + "\"," +
                                  "\"buyr_tel2\":\"" + buyr_tel2 + "\"}";

                // SSL/ TLS 보안 채널을 만들수 없습니다. 오류 발생 시 추가 (프레임워크 버전을 올렸음에도 안될 경우 추가)
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                // API REQ
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(target_URL);
                req.Method = "POST";
                req.ContentType = "application/json";

                byte[] byte_req = Encoding.UTF8.GetBytes(req_param);
                req.ContentLength = byte_req.Length;

                Stream st = req.GetRequestStream();
                st.Write(byte_req, 0, byte_req.Length);
                st.Close();

                // API RES
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader st_read = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                res_param = st_read.ReadToEnd();

                st_read.Close();
                res.Close();

                // RES JSON DATA Parsing
                JObject json_data = JObject.Parse(res_param);
                res_cd = json_data["res_cd"].ToString();
                res_msg = json_data["res_msg"].ToString();

                if (res_cd == "0000")
                {
                    tno = json_data["tno"].ToString();
                    pnt_amount = json_data["pnt_amount"].ToString();
                    pnt_app_time = json_data["pnt_app_time"].ToString();
                    pnt_app_no = json_data["pnt_app_no"].ToString();
                    rsv_pnt = json_data["rsv_pnt"].ToString();
                }
            }

            if (req_tx == "mod") // 취소
            {
                /* =  결제 API URL ============================================================= */
                // target_URL = "https://stg-spl.kcp.co.kr/gw/mod/v1/cancel"; // 개발서버
                target_URL = "https://spl.kcp.co.kr/gw/mod/v1/cancel"; // 운영서버

                // 요청정보
                site_cd = Request.Form["site_cd"];
                mod_type = Request.Form["mod_type"];
                tno = Request.Form["tno"];
                mod_desc = Request.Form["mod_desc"];

                /* =    KCP PG-API 가맹점 테스트용 개인키 READ   ========================================= */
                // PKCS#8 PEM READ
                StreamReader sr = new StreamReader("C:\\Users\\colds\\OneDrive\\바탕 화면\\dotnet_kcp_api_benepia_hub_sample\\dotnet_kcp_api_benepia_hub_sample\\dotnet_kcp_api_benepia_hub_sample\\certificate_agl\\KCP_AUTH_AL83X_PRIKEY.pem"); // 개인키 경로 ("splPrikeyPKCS8.pem" 은 테스트용 개인키)
                String privateKeyText = sr.ReadToEnd();

                // 개인키 비밀번호
                string privateKeyPass = "win$agl2025"; // 개인키 비밀번호 ("changeit" 은 테스트용 개인키 비밀번호)

                // 개인키정보 READ
                StringReader stringReader = new StringReader(privateKeyText);
                PemReader pemReader = new PemReader(stringReader, new PasswordFinder(privateKeyPass));
                RsaPrivateCrtKeyParameters keyParams = (RsaPrivateCrtKeyParameters)pemReader.ReadObject();

                String cancel_target_data = site_cd + "^" + tno + "^" + mod_type;
                System.Diagnostics.Debug.WriteLine(cancel_target_data);

                byte[] tmpSource = Encoding.ASCII.GetBytes(cancel_target_data);

                ISigner sign = SignerUtilities.GetSigner(PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id);
                sign.Init(true, keyParams);
                sign.BlockUpdate(tmpSource, 0, tmpSource.Length);

                var sign_data = sign.GenerateSignature();
                String kcp_sign_data = Convert.ToBase64String(sign_data);

                var req_temp = JObject.Parse("{\"site_cd\" : \"" + site_cd + "\"," +
                                  "\"kcp_cert_info\":\"" + KCP_CERT_INFO + "\"," +
                                  "\"kcp_sign_data\":\"" + kcp_sign_data + "\"," +
                                  "\"mod_type\":\"" + mod_type + "\"," +
                                  "\"tno\":\"" + tno + "\"," +
                                  "\"mod_desc\":\"" + mod_desc + "\"}");

                if (mod_type == "STRA")
                {
                    req_temp.Add("mod_mny", Request.Form["mod_mny"]);
                    req_temp.Add("mod_ordr_idxx", Request.Form["mod_ordr_idxx"]);
                    req_temp.Add("mod_ordr_goods", Request.Form["mod_ordr_goods"]);
                }

                req_param = req_temp.ToString();
                // SSL/ TLS 보안 채널을 만들수 없습니다. 오류 발생 시 추가 (프레임워크 버전을 올렸음에도 안될 경우 추가)
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                // API REQ
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(target_URL);
                req.Method = "POST";
                req.ContentType = "application/json";

                byte[] byte_req = Encoding.UTF8.GetBytes(req_param);
                req.ContentLength = byte_req.Length;

                Stream st = req.GetRequestStream();
                st.Write(byte_req, 0, byte_req.Length);
                st.Close();

                // API RES
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader st_read = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                res_param = st_read.ReadToEnd();

                st_read.Close();
                res.Close();

                // RES JSON DATA Parsing
                JObject json_data = JObject.Parse(res_param);
                res_cd = json_data["res_cd"].ToString();
                res_msg = json_data["res_msg"].ToString();

                if (res_cd == "0000")
                {
                    tno = json_data["tno"].ToString();
                    if (mod_type == "STRA")
                    {
                        pnt_amount = json_data["pnt_amount"].ToString();
                        pnt_app_no = json_data["pnt_app_no"].ToString();
                    }
                }
            }
        }
        private class PasswordFinder : IPasswordFinder
        {
            private string password;
            public PasswordFinder(string pwd)
            {
                password = pwd;
            }
            public char[] GetPassword()
            {
                return password.ToCharArray();
            }
        }
    }
}