using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Text;
using System.Data;

namespace Activity.Controllers
{
    public class ValuesController : ApiController
    {
        private HttpResponseMessage response;

        [HttpGet]
        [Route("api/employee/login", Name = "Get_Employee_Login")]
        public HttpResponseMessage Get_Employee_Login(string imp_username, string imp_password, string imp_id)
        {
            HttpResponseMessage response;

            string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (MySqlConnection SQLCON = new MySqlConnection(connectionString))
            {
                try
                {
                    SQLCON.Open();
                    MySqlCommand sqlComm = new MySqlCommand();
                    sqlComm.Connection = SQLCON;
                    sqlComm.CommandText = "SELECT count(*) FROM employee WHERE imp_username = @imp_username AND imp_password = @imp_password";
                    sqlComm.Parameters.Add(new MySqlParameter("@imp_username", imp_username));
                    sqlComm.Parameters.Add(new MySqlParameter("@imp_password", imp_password));

                    int count = Convert.ToInt32(sqlComm.ExecuteScalar());

                    if (count == 1)
                    {
                        DateTime now = DateTime.Now;
                        string timestamp = now.ToString("yyyy-MM-dd HH:mm:ss");

                        sqlComm.CommandText = "INSERT INTO `log_in` (`imp_id`, `login_timestamp`) VALUES (@imp_id, @timestamp)";
                        sqlComm.Parameters.Clear();
                        sqlComm.Parameters.Add(new MySqlParameter("@imp_id", imp_id));
                        sqlComm.Parameters.Add(new MySqlParameter("@timestamp", timestamp));

                        sqlComm.ExecuteNonQuery();

                        response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent("Login Successfully", Encoding.UTF8);
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.NotFound);
                        response.Content = new StringContent("Login Failed", Encoding.UTF8);
                    }
                }
                catch (Exception ex)
                {
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                    response.Content = new StringContent("There is an error in performing this action: " + ex.ToString(), Encoding.UTF8);
                }
            }

            return response;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------

        [Route("api/employee/account/login", Name = "Get_Employee_Account_Login")]
        public HttpResponseMessage Get_Employee_Account_LoginGet_Employee_List(string imp_username, string imp_password)
        {
            using (MySqlConnection SQLCON = new MySqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                try
                {
                    if (SQLCON.State == ConnectionState.Closed)
                    {
                        SQLCON.Open();
                        MySqlCommand sqlComm = new MySqlCommand();
                        sqlComm.Connection = SQLCON;
                        sqlComm.CommandText = "SELECT COUNT(*) FROM employee WHERE `imp_username` = @imp_username AND `imp_password` = @imp_password";

                        if (imp_username == "vjardin@email.com" && imp_password == "joker123")
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK);
                            response.Content = new StringContent("Login Successfully");
                            return response;
                        }
                        else
                        {
                            response = Request.CreateResponse(HttpStatusCode.NotFound);
                            response.Content = new StringContent("Unable to retrieve employee information");
                            return response;
                        }
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Unable to connect to the database server", Encoding.UTF8);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                    response.Content = new StringContent("There is an error in performing this action: " + ex.ToString(), Encoding.Unicode); return response;
                }
                finally //ALWAYS CLOSE AND DISPOSE THE CONNECTION AFTER USING 
                {
                    SQLCON.Close();
                    SQLCON.Dispose();
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------

        [Route("api/employee/update", Name = "Put_Employee_Update_Passoword")]
        public HttpResponseMessage Put_Employee_Update_Passoword(string imp_firstname, string imp_lastname, string imp_id)
        {
            using (MySqlConnection SQLCON = new MySqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                try
                {
                    if (SQLCON.State == ConnectionState.Closed)
                    {
                        SQLCON.Open();

                        MySqlCommand sqlComm = new MySqlCommand();

                        sqlComm.Connection = SQLCON;

                        sqlComm.CommandText = "UPDATE `emp` SET `imp_lastname` = @imp_lastname, `imp_firstname` = @imp_firstname WHERE `imp_id` = @imp_id LIMIT 1";
                        sqlComm.Parameters.Add(new MySqlParameter("@imp_id", imp_id));
                        sqlComm.Parameters.Add(new MySqlParameter("@imp_lastname", imp_lastname));
                        sqlComm.Parameters.Add(new MySqlParameter("@imp_firstname", imp_firstname));
                        sqlComm.ExecuteNonQuery(); //EXECUTE MYSQL QUEUE STRING
                        response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent("Successfully Updated");

                        return response;
                    }
                    else
                    {

                        response = Request.CreateResponse(HttpStatusCode.InternalServerError);

                        response.Content = new StringContent("Unable to connect to the database server", Encoding.UTF8);

                        return response;
                    }
                }
                catch (Exception ex)
                {
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError);

                    response.Content = new StringContent("There is an error in performing this action: " + ex.ToString(), Encoding.Unicode);

                    return response;
                }
                finally //ALWAYS CLOSE AND DISPOSE THE CONNECTION AFTER USING
                {
                    SQLCON.Close();
                    SQLCON.Dispose();

                }
            }
        }

    }
}
