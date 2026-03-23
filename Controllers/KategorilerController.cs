using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneProjesi.Data;
using KutuphaneProjesi.Models;

namespace KutuphaneProjesi.Controllers
{
    public class KategorilerController : Controller
    {
        private readonly KutuphaneContext _context;
        public KategorilerController(KutuphaneContext context) => _context = context;

        public async Task<IActionResult> Index() => View(await _context.Kategoriler.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create(Kategori kategori) {
            _context.Add(kategori); await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}