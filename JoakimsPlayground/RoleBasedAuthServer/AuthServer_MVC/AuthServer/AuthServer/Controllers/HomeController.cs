//using System.Diagnostics;
//using AuthServer.Data.Context;
//using AuthServer.Data.Entities;
//using AuthServer.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace AuthServer.Controllers
//{
//    public class HomeController : Controller
//    {
//        private readonly ILogger<HomeController> _logger;

//        private readonly AuthServerDbContext _context;

//        public HomeController(ILogger<HomeController> logger, AuthServerDbContext context)
//        {
//            _logger = logger;
//            _context = context;
//		}

//        public IActionResult Index()
//        {
//            return View();
//        }

//   //     public IActionResult Expenses()
//   //     {
//   //         var allExpenses = _context.Expenses.ToList();

//			//return View(allExpenses);
//   //     }

//		//public IActionResult CreateEditExpense(int? id)
//		//{
//  //          if (id != null)
//  //          {
//		//		var expense = _context.Expenses.SingleOrDefault(e => e.Id == id);

//  //              var expenseDto = new ExpenseDto
//  //              {
//  //                  Id = expense.Id,
//  //                  Description = expense.Description,
//  //                  Value = expense.Value
//  //              };  

//		//		return View(expenseDto);
//		//	}

//		//	return View();
//		//}

//		//public IActionResult DeleteExpense(int? id)
//		//{
//  //          var expense = _context.Expenses.SingleOrDefault(e => e.Id == id);
//  //          if (expense != null)
//  //          {
//  //              _context.Expenses.Remove(expense);
//  //              _context.SaveChanges();
//		//	}

//		//	return RedirectToAction("Expenses");
//		//}

//		//public IActionResult CreateEditExpenseForm(ExpenseDto expenseModel)
//  //      {
//		//	var entity = new Expense
//		//	{
//		//		Id = expenseModel.Id,
//		//		Description = expenseModel.Description,
//		//		Value = expenseModel.Value
//		//	};

//		//	if (entity.Id == 0)
//  //          {
//		//		// Create
//		//		_context.Expenses.Add(entity);
//		//	}
//  //          else
//  //          {
//  //              // Edit
//		//		_context.Expenses.Update(entity);
//		//	}

//		//	_context.SaveChanges();

//		//	return RedirectToAction("Expenses");
//		//}

//		//public IActionResult Privacy()
//  //      {
//  //          return View();
//  //      }

//  //      [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//  //      public IActionResult Error()
//  //      {
//  //          return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//  //      }
//    }
//}
