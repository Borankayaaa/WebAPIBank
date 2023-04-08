using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPIBank_0.DesignPatterns.SingletonPattern;
using WebAPIBank_0.Models.Context;
using WebAPIBank_0.Models.Entities;
using WebAPIBank_0.RequestModel;
using WebAPIBank_0.ResponseModel;

namespace WebAPIBank_0.Controllers
{
    public class PaymentController : ApiController
    {
        MyContext _db;

        public PaymentController()
        {
            _db = DBTool.DBInstance;
        }

        //Asagıdaki Action sadece development testi icindir...API canlıya cıkacagı zaman kesinlikle acık bırakılmamalıdır...

        //[HttpGet]
        //public List<PaymentResponseModel> GetAll()
        //{
        //    return _db.Cards.Select(x => new PaymentResponseModel
        //    {
        //        CardExpiryMonth = x.CardExpiryMonth,
        //        CardExpiryYear = x.CardExpiryYear,
        //        CardNumber = x.CardNumber
        //    }).ToList();
        //}

        [HttpPost]
        public IHttpActionResult ReceivePayment(PaymentRequestModel item)
        {
            CardInfo ci = _db.Cards.FirstOrDefault(x => x.CardNumber == item.CardNumber && x.SecurityNumber == item.SecurityNumber && x.CardUserName == item.CardUserName && x.CardExpiryYear == item.CardExpiryYear && item.CardExpiryMonth == item.CardExpiryMonth);


            if (ci != null)
            {
                if (ci.CardExpiryYear < DateTime.Now.Year)
                {
                    return BadRequest("Expired Card");
                }
                else if (ci.CardExpiryYear == DateTime.Now.Year)
                {
                    if (ci.CardExpiryMonth < DateTime.Now.Month)
                    {

                        return BadRequest("Expired Card(Month)");
                    }

                    if(ci.Balance >= item.ShoppingPrice)
                    {
                        SetBalance(item, ci);
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Balance exceeded");
                    }
                }

                else if(ci.Balance >= item.ShoppingPrice)
                {
                    SetBalance(item, ci);
                    return Ok();
                }

                return BadRequest("Balance exceeded");
            }

            return BadRequest("Card Info Wrong");
        }



        private void SetBalance(PaymentRequestModel item,CardInfo ci)
        {
            ci.Balance -= item.ShoppingPrice;
            //ShoppingPrice'tan yüzdelik komisyon alınıp  kalan miktar alacaklının hesabına aktarılır...
            _db.SaveChanges();
        }

    }
}
