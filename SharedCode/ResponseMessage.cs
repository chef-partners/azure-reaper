using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Azure.Reaper
{
    public class ResponseMessage
    {
        private Dictionary<string, dynamic> _message = new Dictionary<string, dynamic>();
        private bool _is_error = false;
        private HttpStatusCode _http_status_code = HttpStatusCode.OK;

        public ResponseMessage(string message, bool error, HttpStatusCode code)
        {
            SetMessage(message);
            _is_error = error;
            _http_status_code = code;
        }

        public ResponseMessage()
        { }

        public bool IsError()
        {
            return _is_error;
        }

        public void SetError()
        {
            _is_error = true;
        }

        public void SetError(bool error = true)
        {
            _is_error = error;
        }

        public void SetError(string message, bool error, HttpStatusCode code)
        {
            SetMessage(message);
            _is_error = error;
            _http_status_code = code;
        }

        public void SetMessage(string message)
        {
            if (_message.ContainsKey("message"))
            {
                _message["message"] = message;
            }
            else
            {
                _message.Add("message", message);
            }
        }

        public void SetStatusCode(HttpStatusCode code)
        {
            _http_status_code = code;
        }

        public HttpResponseMessage CreateResponse(dynamic data = null)
        {
            string content;

            if (data == null)
            {
                _message.Add("error", _is_error);
                content = JsonConvert.SerializeObject(_message);
            }
            else
            {
                if (_is_error)
                {
                    _message.Add("error", true);
                    content = JsonConvert.SerializeObject(_message);
                }
                else
                {
                    content = JsonConvert.SerializeObject(data);
                }
            }

            return new HttpResponseMessage(_http_status_code)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
        }

    }
}
