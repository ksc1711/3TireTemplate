using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using WCMS.FrameWork.Data.Mapper.Dapper;
using WCMS.FrameWork.Data.MsSql;

namespace WCMS.FrameWork.Logging
{
    // TODO : 1. SMS 발송 기능 추가
    // TODO : 2. Email 발송 기능 추가
    public sealed class LogHelper
    {
        private static readonly LogHelper _instance = null;
        private string _logPath = string.Empty;

        #region [Properties]
        private bool _bDB = false;
        private bool _bSMS = false;
        private bool _bEmail = false;
        private bool _bEvent = false;
        private string _LogFileDir = "LogFileDir";


        /// <summary>
        /// 로그 DB 저장 유무
        /// 기본값 : false
        /// </summary>
        public bool IsDB
        {
            get { return _bDB; }
            set { _bDB = value; }
        }

        /// <summary>
        /// 로그 SMS 발송 여부
        /// 기본값 : false
        /// </summary>
        public bool IsSMS
        {
            get { return _bSMS; }
            set { _bSMS = value; }
        }

        /// <summary>
        /// 로그 Email 발송 유무
        /// 기본값 : false
        /// </summary>
        public bool IsEmail
        {
            get { return _bEmail; }
            set { _bEmail = value; }
        }

        /// <summary>
        /// 로그 이벤트로그 저장 유무
        /// 기본값 : false
        /// </summary>
        public bool IsEvent
        {
            get { return _bEvent; }
            set { _bEvent = value; }
        }

        /// <summary>
        /// LogFileDir
        /// </summary>
        public string LogFileDir
        {
            get { return _LogFileDir; }
            set { _LogFileDir = value;
              setLogPath(_LogFileDir);
            }
        }
        #endregion

        /// <summary>
        /// Singleton LogHelper 객체 생성
        /// </summary>
        public static LogHelper Instance {
            get {
                return _instance;
            }
        }
   

        static LogHelper() {
            _instance = new LogHelper();
        }

        // Constructor as Private
        private LogHelper(string LogFileDir = "") {
            setLogPath(LogFileDir);
        }

        private void setLogPath(string LogFileDir)
        {
            if (LogFileDir == "") LogFileDir = "LogFileDir";
            _logPath = MakeLogFileName(ConfigurationManager.AppSettings[LogFileDir].ToString());
        }


        /// <summary>
        /// 로그 파일명 생성
        /// web.config에 선언된 로그 파일 전체 경로를 읽어와 해당 년월일 조합 파일명을 리턴해준다.
        /// ex: <add key="LogFileDir" value="D:\ComLog\MTPartner\MWebAPI_PLAYDB.txt"/>
        /// </summary>
        /// <param name="strFileName">로그파일FullName(경로포함)</param>
        /// <returns></returns>
        private string MakeLogFileName(string strFileName)
        {
            return string.Format("{0}\\{1}_{2}{3}", Path.GetDirectoryName(strFileName), Path.GetFileNameWithoutExtension(strFileName), DateTime.Now.ToString("yyyyMMdd"), Path.GetExtension(strFileName));
        }

        /// <summary>
        /// 로그메세지 생성
        /// </summary>
        /// <param name="errMsg">에러메세지</param>
        /// <returns></returns>
        private string LogMessage(string errMsg)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("==============================");
            sb.AppendLine(DateTime.Now.ToString());
            sb.AppendLine("==============================");
            sb.AppendLine(errMsg);

            return sb.ToString();
        }


        /// <summary>
        /// 로그메세지 생성
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="controlName">컨트롤러명</param>
        /// <param name="actionName">액션명</param>
        /// <returns></returns>
        private string LogMessage(Exception ex, string controlName = "", string actionName = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("==============================");
            sb.AppendLine(DateTime.Now.ToString());
            sb.AppendLine("==============================");
            sb.AppendLine(string.Format("[RECV]Controller:{0}  Action:{1}", controlName, actionName));
            sb.AppendLine(string.Format("[ERROR]{0}{2}[TRACE]{1}", ex.Message, ex.StackTrace, Environment.NewLine));

            return sb.ToString();
        }


        private string LogMessage(string message, string controlName = "", string actionName = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("==============================");
            sb.AppendLine(DateTime.Now.ToString());
            sb.AppendLine("==============================");
            sb.AppendLine(string.Format("[RECV]Controller:{0}  Action:{1}", controlName, actionName));
            sb.AppendLine(string.Format("[Msg]{0}{1}", message, Environment.NewLine));

            return sb.ToString();
        }        


        public void LogWrite(string strMsg)
        {
            LogWrite(new Exception(strMsg), "", "", "");
        }

        public void LogWrite(Exception ex)
        {
            LogWrite(ex, "", "", "");
        }

        public void LogWrite(string message, object[] args)
        {
            string formattedMessage = string.Format(CultureInfo.InvariantCulture, message, args);
            LogDebug(formattedMessage);
        }

        /// <summary>
        /// 로그 파일 생성
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="controlName"></param>
        /// <param name="actionName"></param>
        /// <param name="IP"></param>
        public void LogWrite(Exception ex, string controlName = "", string actionName = "", string IP = "")
        {
            string errorMessage = LogMessage(ex, controlName, actionName);
            try
            {
                StreamWriter streamWriter = new StreamWriter(_logPath, true);
                streamWriter.Write(errorMessage);
                streamWriter.Close();

                if (_bDB) LogWriteToDB(ex, controlName, actionName, IP);
                if (_bSMS) LogWriteToSMS(ex, controlName, actionName, IP);
                if (_bEmail) LogWriteToEmail(ex, controlName, actionName, IP);
                if (_bEvent) LogWriteToEvent(errorMessage);
            }catch (Exception) { }
        }

        public void LogWrite(string controlName = "", string actionName = "", string message = "")
        {
            string errorMessage = LogMessage(message, controlName, actionName);
            try
            {
                StreamWriter streamWriter = new StreamWriter(_logPath, true);
                streamWriter.Write(errorMessage);
                streamWriter.Close();

                //if (_bDB) LogWriteToDB(ex, controlName, actionName, IP);
                //if (_bSMS) LogWriteToSMS(ex, controlName, actionName, IP);
                //if (_bEmail) LogWriteToEmail(ex, controlName, actionName, IP);
                //if (_bEvent) LogWriteToEvent(errorMessage);
            }
            catch (Exception) { }
        }


        public void LogDebug(string message = "")
        {
            try
            {
                string errorMessage = string.Empty;


                if (message == "")
                {
                    errorMessage = LogMessage(LogCreate());
                }
                else
                {
                    errorMessage = LogMessage(message);
                }


                StreamWriter streamWriter = new StreamWriter(_logPath, true);
                streamWriter.Write(errorMessage);
                streamWriter.Close();
            }
            catch (Exception) { }
        }

        public string LogCreate()
        {

            NameValueCollection coll = HttpContext.Current.Request.Params;
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("QUERY_STRING : {0} , POST : ", HttpContext.Current.Request.ServerVariables["QUERY_STRING"]));

            for (int i = 0; i < coll.Count; i++)
            {
                if (coll.GetKey(i) != "__VIEWSTATE")
                {
                    string Query = string.Format("{0}={1}&", coll.GetKey(i), coll.GetValues(i)[0]);
                    sb.Append(Query);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// DB 저장
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="controlName"></param>
        /// <param name="actionName"></param>
        /// <param name="IP"></param>
        private void LogWriteToDB(Exception ex, string controlName, string actionName, string IP) 
        {
            try
            {
                using (DBHelper dbUtil = new DBHelper("LogDBConnection"))
                {
                   // string query  = "Insert tErrorLog (ControllerName, ActionName, ErrorMessage, StackTrace, IP)  values ({0}, {1}, {2}, {3}, {4})";
					//dbUtil.ExecuteNonQuery(query, controlName, actionName, ex.Message, ex.StackTrace ?? "", IP);

					dbUtil.AddParameter("ControllerName", controlName);
					dbUtil.AddParameter("ActionName", actionName);
					dbUtil.AddParameter("ErrorMessage", ex.Message);
					dbUtil.AddParameter("StackTrace", ex.StackTrace ?? "");
					dbUtil.AddParameter("IP", IP);
					dbUtil.ExecuteNonQuery(System.Data.CommandType.StoredProcedure, "Proc_Web_SetErrorLog");
                }
            }catch (Exception) { }
        }

        /// <summary>
        /// SMS 발송
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="controlName"></param>
        /// <param name="actionName"></param>
        /// <param name="IP"></param>
        private void LogWriteToSMS(Exception ex, string controlName, string actionName, string IP)
        {
            try
            {
                // pBack_SMS_UserRegister
                //@vSMSType   : 등록채널구분
                //@vMemNo	  : 회원번호
                //@vMobile	  : 핸드폰번호
                //@vContents  : 내용
                //@vMemberCode: 회원번호
                //@vReserved2 : 예약2
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Email 발송
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="controlName"></param>
        /// <param name="actionName"></param>
        /// <param name="IP"></param>
        private void LogWriteToEmail(Exception ex, string controlName, string actionName, string IP)
        {
            try
            {
                #region smtp 서버를 구축한 경우
                //string _senderID = "발신자 이메일";
                //string _senderName = "발신자";
                //string _title = "제목";
                //string _body = "내용";

                //MailMessage _message = new MailMessage();
                //_message.From = new MailAddress(_senderID, _senderName, System.Text.Encoding.UTF8);
                //_message.To.Add("수신자 이메일");
                //_message.Subject = _title;
                //_message.SubjectEncoding = System.Text.Encoding.UTF8;
                //_message.Body = _body;
                //_message.IsBodyHtml = true;  //내용에 html이 포함된 경우

                //SmtpClient server = new SmtpClient("ip", port);
                //server.UseDefaultCredentials = false;
                //server.EnableSsl = false;  //SSL을 설정하지 않은 경우
                //server.Send(_message);
                #endregion

                #region 구글 smtp를 이용한 경우
                //string _senderID = "발신자 이메일";
                //string _senderName = "발신자";
                //string _title = "제목";
                //string _body = "내용";
 
                //MailMessage _message = new MailMessage();
                //_message.From = new MailAddress(_senderID, _senderName, System.Text.Encoding.UTF8);
                //_message.To.Add("수신자 이메일");
                //_message.Subject = _title;
                //_message.SubjectEncoding = System.Text.Encoding.UTF8;
                //_message.Body = _body;
                //_message.IsBodyHtml = true;  //내용에 html이 포함된 경우
 
 
                //SmtpClient server = new SmtpClient("smtp.gmail.com", 587);
                //server.UseDefaultCredentials = false;
                //server.EnableSsl = true;  //google smtp는 ssl의 설정되 있으므로 true
                //server.DeliveryMethod = SmtpDeliveryMethod.Network;
                //server.Credentials = new System.Net.NetworkCredential("구글 아이디", "패스워드");
                //server.Send(_message);
                #endregion
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 이벤트로그 저장
        /// </summary>
        /// <param name="errorMessage"></param>
        private void LogWriteToEvent(string errorMessage)
        {
            try
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry(errorMessage, EventLogEntryType.Error, 101);
                } 
            }
            catch (Exception) { }
        }
    }
}
