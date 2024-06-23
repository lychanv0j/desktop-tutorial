using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using nssets.Models;
using System.Data.Entity;

public class HomeController : Controller{
    private readonly Model1 _db; 
    public HomeController()
    {
        _db = new Model1();
    }


    public ActionResult Index()
    {
        var products = _db.Products.ToList(); 
        return View(products);
    }

    public ActionResult IndexLogin()
    {
        if (Session["UserID"] != null)
        {
            return View();
        }
        else
        {
            return RedirectToAction("Login");
        }
    }



//GET: Register

public ActionResult Register()
    {
        return View();
    }

    //POST: Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Register(Customer user)
    {

    int maxId = _db.Customers.Any() ? _db.Customers.Max(c => c.CustomerID) : 0;
    user.CustomerID = maxId + 1;

    if (ModelState.IsValid)
        {
            var check = _db.Customers.FirstOrDefault(s => s.Email == user.Email);
            if (check == null)
            {
                user.Password = GetMD5(user.Password);
                _db.Configuration.ValidateOnSaveEnabled = false;
                _db.Customers.Add(user);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.error = "Email already exists";
                return View();
            }


        }
        return View();


    }

    public ActionResult Login()
    {
        return View();
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Login(string username, string password)
    {
        var f_password = GetMD5(password);
        var users = _db.Customers.Where(s => s.Username.Equals(username) && s.Password.Equals(f_password)).ToList();

        if (users.Any())
        {
            foreach (var user in users)
            {
                if (user != null)
                {
                    //add session
                    Session["UserID"] = user.CustomerID;
                    Session["Username"] = user.Username;
                    Session["Email"] = user.Email;
                    
                    var redirectUrl = Session["RedirectUrl"];
                    if (redirectUrl != null)
                    {
                        Session["RedirectUrl"] = null;
                        return Redirect(redirectUrl.ToString());
                    }
                    return RedirectToAction("Index", new { id = user.CustomerID});
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login");
                }
            }
        }
        return View();
    }


    //Logout
    public ActionResult Logout()
    {
        Session.Clear();//remove session
        return RedirectToAction("Login");
    }



    //create a string MD5
    public static string GetMD5(string str)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] fromData = Encoding.UTF8.GetBytes(str);
        byte[] targetData = md5.ComputeHash(fromData);
        string byte2String = null;

        for (int i = 0; i < targetData.Length; i++)
        {
            byte2String += targetData[i].ToString("x2");

        }
        return byte2String;
    }






    public ActionResult About()
    {
        ViewBag.Message = "Your application description page.";

        return View("About");
    }

    public ActionResult Contact()
    {
        ViewBag.Message = "Your contact page.";

        return View("Contact");
    }

        


    

    public ActionResult Product()
    {
        ViewBag.Message = "Your contact page.";

        return View("");
    }

    public ActionResult HotProduct()
    {

        var hotProduct = _db.Products.Where(p => p.HotProduct == true).ToList();
        ViewBag.Message = "Your contact page.";

        return View(hotProduct);
    }
    public ActionResult bestSaller()
    {
        //var bestSaller = _db.Products.Where(p => p.BestSeller == true).ToList();
        //ViewBag.Message = "Your contact page.";
        var hotproducts = _db.Products.Where(p => p.HotProduct == true).ToList();
        return View(hotproducts);
    }
    public ActionResult ProductDetail(int id)
    {
        ViewBag.ProductID = id;
        var productDT = _db.ProductDetailPages.ToList();
        return View(productDT); // Truyền model tới view
    }


       

    private int GetCustomerIdFromSession()
    {

        return (int)Session["UserID"];
    }

 
    //
    

    public ActionResult Cart()
    {
        // Lấy ID của khách hàng từ Session
        int customerId = (int)Session["UserID"];

        // Lấy tất cả các mục trong giỏ hàng của khách hàng
        var cartItems = _db.Carts.Where(c => c.CustomerID == customerId).ToList();

        // Truyền dữ liệu giỏ hàng đến view
        return View(cartItems);
    }

    [HttpPost]
    public ActionResult AddToCart(int productId, int quantity)
    {
        // Lấy ID của khách hàng từ Session
        int customerId = (int)Session["UserID"];
       
        // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
        var existingCart = _db.Carts.FirstOrDefault(c => c.ProductID == productId && c.CustomerID == customerId);

        if (existingCart != null)
        {
            // Nếu có, cập nhật số lượng
            existingCart.Quantity += quantity;
            _db.Entry(existingCart).State = EntityState.Modified;
        }
        else
        {
            // Nếu chưa, thêm sản phẩm mới vào giỏ
            var newCart = new Cart
            {
                CustomerID = customerId,
                ProductID = productId,
                Quantity = quantity
            };
            _db.Carts.Add(newCart);
        }

        // Lưu thay đổi vào database

            _db.SaveChanges();
            

        // Chuyển hướng người dùng về trang xem giỏ hàng hoặc tiếp tục mua sắm
        return RedirectToAction("Cart");
    }
    //
    public ActionResult AddProduct(int productId)
    {
        if (Session["Username"] == null)
        {
            return RedirectToAction("Login", "Account"); // Redirect to login if user is not logged in
        }

        int customerId = (int)Session["UserID"]; // Assuming CustomerID is stored in session

        var cartItem = _db.Carts.FirstOrDefault(c => c.ProductID == productId && c.CustomerID == customerId);
        if (cartItem != null)
        {
            cartItem.Quantity += 1;
            _db.Entry(cartItem).State = EntityState.Modified;
        }
        else
        {
            cartItem = new Cart
            {
                ProductID = productId,
                Quantity = 1,
                CustomerID = customerId
            };
            _db.Carts.Add(cartItem);
        }
        _db.SaveChanges();

        return RedirectToAction("Cart"); // Redirect back to the cart view
    }
    public ActionResult DropProduct(int productId)
    {
        if (Session["Username"] == null)
        {
            return RedirectToAction("Login", "Account");
        }

        int customerId = (int)Session["UserID"];

        var cartItem = _db.Carts.FirstOrDefault(c => c.ProductID == productId && c.CustomerID == customerId);
        if (cartItem != null && cartItem.Quantity > 1)
        {
            cartItem.Quantity -= 1;
            _db.Entry(cartItem).State = EntityState.Modified;
            _db.SaveChanges();
        }

        return RedirectToAction("Cart");
    }
    public ActionResult RemoveProduct(int productId)
    {
        if (Session["Username"] == null)
        {
            return RedirectToAction("Login", "Account");
        }

        int customerId = (int)Session["UserID"];

        var cartItem = _db.Carts.FirstOrDefault(c => c.ProductID == productId && c.CustomerID == customerId);
        if (cartItem != null)
        {
            _db.Carts.Remove(cartItem);
            _db.SaveChanges();
        }
        return RedirectToAction("Cart");
    }
    public ActionResult MyAccont()
    {
        return View ();
    }
}
