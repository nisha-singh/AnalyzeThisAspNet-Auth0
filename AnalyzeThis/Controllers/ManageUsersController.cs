using AnalyzeThis.Models;
using Auth0;
using JWT;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace AnalyzeThis.Controllers
{
    public class ManageUsersController : Controller
    {
        private readonly Client _client;
        private readonly Client _auth0;
        private bool emailsent = false;

        public ManageUsersController()
        {
            _client = new Client(
                ConfigurationManager.AppSettings["auth0:ClientId"],
                ConfigurationManager.AppSettings["auth0:ClientSecret"],
                ConfigurationManager.AppSettings["auth0:Domain"]);

            
        }


        // GET: ManageUsers
        public ActionResult Index()
        {
            return View();
        }

        // GET: ManageUsers/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ManageUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ManageUsers/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            string firstname =  Request["firstname"].ToString();
            string lastname = Request["lastname"].ToString();
            string email = Request["email"].ToString();

            var randomPassword = Guid.NewGuid().ToString();
            var metadata = new
            {
                firstname,
                lastname,
                activation_pending = true
            };

                var profile = _client.CreateUser(email, randomPassword,
                      ConfigurationManager.AppSettings["auth0:Connection"], false, metadata);


            var userToken = JWT.JsonWebToken.Encode(
                new { id = profile.UserId, email = profile.Email },
                  ConfigurationManager.AppSettings["analyze:signingKey"],
                    JwtHashAlgorithm.HS256);

            var verificationUrl = _client.GenerateVerificationTicket(profile.UserId,
                  Url.Action("Activate", "ManageUsers", new { area = "", userToken }, Request.Url.Scheme));

            var body = "Hello {0}, " +
              "Great that you're using our application. " +
              "Please click <a href='{1}'>ACTIVATE</a> to activate your account." +
              "The AnalyzeThis team!";

            var fullName = String.Format("{0} {1}", firstname, lastname).Trim();
            var mail = new MailMessage("myapp@auth0.com", email, "Hello there!",
                String.Format(body, fullName, verificationUrl));
            mail.IsBodyHtml = true;

            
            var mailClient = new SmtpClient();
            //mailClient.Credentials = new NetworkCredential();
            mailClient.Send(mail);


          return RedirectToAction("Index");

        }

       
        public ActionResult Activate(string userToken)
        {
            dynamic metadata = JWT.JsonWebToken.DecodeToObject(userToken,
                ConfigurationManager.AppSettings["analyze:signingKey"]);
            var user = _client.GetUser(metadata["id"]);
            if (user != null)
                return View(new UserActivationModel { Email = user.Email, UserToken = userToken });
            return View("ActivationError",
                new UserActivationErrorModel("Error activating user, could not find an exact match for this email address."));
        }



        /// <summary>
        /// POST Account/Activate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Activate(UserActivationModel model)
        {
            dynamic metadata = JWT.JsonWebToken.DecodeToObject(model.UserToken,
                ConfigurationManager.AppSettings["analyze:signingKey"], true);
            if (metadata == null)
            {
                return View("ActivationError",
                    new UserActivationErrorModel("Unable to find the token."));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            UserProfile user = _client.GetUser(metadata["id"]);
            if (user != null)
            {
                if (user.ExtraProperties.ContainsKey("activation_pending")
                      && !((bool)user.ExtraProperties["activation_pending"]))
                    return View("ActivationError",
                      new UserActivationErrorModel("Error activating user, the user is already active."));

                _client.ChangePassword(user.UserId, model.Password, false);
                _client.UpdateUserMetadata(user.UserId, new { activation_pending = false });

                return View("Activated");
            }

            return View("ActivationError",
                new UserActivationErrorModel("Error activating user, could not find an exact match for this email address."));
        }

        // GET: ManageUsers/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ManageUsers/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ManageUsers/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ManageUsers/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
