using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Configuration;
using MyData;

using System.Linq.Expressions;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;

namespace WebAPI.Classes
{
    public static class Util
    {
        public static string SendEmail(string toEmailAddress,  string subject, string body)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add(toEmailAddress);
                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["DefaultEmailFromAddress"].ToString());
                mailMessage.Subject = subject.Trim();
                mailMessage.Body = body;
                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"].ToString());

                smtpClient.EnableSsl = true;
                smtpClient.Port = Convert.ToInt16(ConfigurationManager.AppSettings["SMTPServerPort"].ToString());
                smtpClient.UseDefaultCredentials = false;
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["DefaultEmailFromAddress"].ToString(), ConfigurationManager.AppSettings["DefaultEmailFromAddressPassword"].ToString());
                smtpClient.Credentials = credentials;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(mailMessage);
                return "success";
            }
            catch (Exception ex)
            {
                Util.LogError("Util.SendEMail() Error: " + ex.ToString());
                return ("error: " + ex.Message);
            }
        }

        public static string SendEmailToUserID (string toUserID, string subject, string body)
        {

            try
            {

                using (AcmeEntities entities = new AcmeEntities())
                {
                    //resolve email addresses
                    var recipientUserEntity = entities.vw_User_UserProfile.Where(e => e.UserID == toUserID).Select(e => new { e.Email }).FirstOrDefault();
                    //var senderUserEntity = entities.vw_User_UserProfile.Where(e => e.UserID == fromUserID).Select(e => new { e.Email }).FirstOrDefault();

                    return SendEmail(recipientUserEntity.Email, subject, body);

                }
            }
            catch (Exception ex)
            {
                Util.LogError("Util.SendEmailFromTemplate() Error: " + ex.ToString());
                return ("error: " + ex.Message);
            }

        }


        //public static Stream GenerateAvatar(string firstName, string lastName, string userID)
        public static void GenerateAvatar(string firstName, string lastName, string userID)
        {
            try
            {
                List<string> _BackgroundColours = new List<string> { "034078", "ED4715", "EDA600", "8C1313", "3C1642" };

                string firstInitial = "";
                string lastInitial = "";
                try { firstInitial = firstName[0].ToString().ToUpper(); } catch { }
                try { lastInitial = lastName[0].ToString().ToUpper(); } catch { }

                if (firstInitial.Length < 1 && lastInitial.Length < 1)
                {
                    firstInitial = "?"; //user will have a '?' in their avatar if no first and last name provided
                }

                var avatarString = string.Format("{0}{1}", firstInitial, lastInitial);

                var randomIndex = new Random().Next(0, _BackgroundColours.Count - 1);
                var bgColour = _BackgroundColours[randomIndex];

                //var bmp = new Bitmap(192, 192);
                var bmp = new Bitmap(500, 500);
                var sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                //var font = new Font("Arial", 48, FontStyle.Bold, GraphicsUnit.Pixel);
                var font = new Font("Arial", 180, FontStyle.Bold, GraphicsUnit.Pixel);
                var graphics = Graphics.FromImage(bmp);

                graphics.Clear((Color)new ColorConverter().ConvertFromString("#" + bgColour));
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                //graphics.DrawString(avatarString, font, new SolidBrush(Color.WhiteSmoke), new RectangleF(0, 0, 192, 192), sf);
                graphics.DrawString(avatarString, font, new SolidBrush(Color.WhiteSmoke), new RectangleF(0, 0, 500, 500), sf);
                graphics.Flush();


                var filePath = ConfigurationManager.AppSettings["AvatarDirectory"].ToString() + userID + "\\";
                System.IO.Directory.CreateDirectory(filePath);

                //save all sizes
                bmp.Save((filePath + userID + ".png"), ImageFormat.Png);

                //Bitmap bmpMD = new Bitmap(bmp, new Size(300, 300));
                //bmpMD.Save((filePath + "md_" + userID + ".png"));
                //bmpMD.Dispose();

                Bitmap bmpSM = new Bitmap(bmp, new Size(100, 100));
                bmpSM.Save((filePath + "sm_" + userID + ".png"));
                bmpSM.Dispose();

                //Bitmap bmpTN = new Bitmap(bmp, new Size(50, 50));
                //bmpTN.Save((filePath + "tn_" + userID + ".png"));
                //bmpTN.Dispose();
            } catch (Exception ex)
            {
                LogError(ex.ToString());
            }
        }




        public static void LogError(string error)
        {
            try
            {
                using (AcmeEntities entities = new AcmeEntities())
                {
                    Error err = new Error();
                    err.ErrorMessage = error;
                    err.DateCreated = DateTime.UtcNow;
                    entities.Errors.Add(err);
                    entities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
               // throw ex;
            }
        }

        public static void LogError(string error, string userId)
        {
            try
            {
                using (AcmeEntities entities = new AcmeEntities())
                {
                    Error err = new Error();
                    err.ErrorMessage = error;
                    err.DateCreated = DateTime.UtcNow;
                    err.UserID = userId;
                    entities.Errors.Add(err);
                    entities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
               // throw ex;
            }
        }

        public static void LogUnauthorizedError(string error, string userId)
        {
            try
            {
                using (AcmeEntities entities = new AcmeEntities())
                {
                    Error err = new Error();
                    err.ErrorMessage = "UNAUTHORIZED: " + error;
                    err.DateCreated = DateTime.UtcNow;
                    err.UserID = userId;
                    entities.Errors.Add(err);
                    entities.SaveChanges();

                    SendEmail(ConfigurationManager.AppSettings["AdminEmailAddress"].ToString(), "UNAUTHORIZED Attempt", "Error ID: " + err.ID + "\r\n" + err.ErrorMessage);
                }
                
            }
            catch (Exception ex)
            {
                // throw ex;
            }
        }



    }
}