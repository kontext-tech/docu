using Kontext.Data;
using Kontext.Data.Models;
using Kontext.Data.Models.ViewModels;
using Kontext.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Kontext.Docu.Web.Portals.Areas.IdentityAdminArea.Controllers
{
    [Area("IdentityAdminArea")]
    [Authorize]
    public class EmailMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmailMessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(ApplicationAuthorizationPolicies.ManageEmailsPolicy)]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var entities = _context.EmailMessages.OrderByDescending(b => b.DateCreated);
            var pagedList = await entities.ToPagedListAsync(page, pageSize);
            var pagedVM = new PagedViewModel<object, EmailMessage>(null, pagedList, pagedList.TotalItemCount, pagedList.PageCount, pageSize, page);

            return View(pagedVM);
        }

        [Authorize(ApplicationAuthorizationPolicies.ManageEmailsPolicy)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailMessage = await _context.EmailMessages
                .SingleOrDefaultAsync(m => m.EmailMessageId == id);
            if (emailMessage == null)
            {
                return NotFound();
            }

            return View(emailMessage);
        }
    }
}
