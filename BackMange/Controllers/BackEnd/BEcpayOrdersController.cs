using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BackMange.Models;
using Microsoft.AspNetCore.Authorization;

namespace BackMange.Controllers.BackEnd
{
    [Authorize(Roles = "Admin")]
    public class BEcpayOrdersController : Controller
    {
        private readonly GameCaseContext _context;

        public BEcpayOrdersController(GameCaseContext context)
        {
            _context = context;
        }

        // GET: BEcpayOrders
        public async Task<IActionResult> Index(string txtKeyword, int pageNumber = 1)
        {
            // 設定每頁顯示筆數
            int pageSize = 10;

            // 準備基礎查詢
            var query = _context.TEcpayOrders
                .Include(t => t.Fposter)
                .Include(t => t.Ftask)
                .Include(t => t.Fworker)
                .AsQueryable();

            // 搜尋條件
            if (!string.IsNullOrEmpty(txtKeyword))
            {
                txtKeyword = txtKeyword.Trim();
                query = query.Where(o =>
                    o.MerchantTradeNo.Contains(txtKeyword) ||
                    o.MemberId.Contains(txtKeyword) ||
                    o.Fposter.FfullName.Contains(txtKeyword) ||
                    o.Fworker.FfullName.Contains(txtKeyword) ||
                    o.Ftask.Ftitle.Contains(txtKeyword) ||
                    // 搜尋付款狀態
                    (txtKeyword == "付款成功" && o.RtnCode == 1) ||
                    (txtKeyword == "待付款" && o.RtnCode == 0) ||
                    (txtKeyword == "付款失敗" && o.RtnCode != 1 && o.RtnCode != 0)
                );
            }

            // 計算分頁
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // 取得當前頁面的資料
            var orders = await query
                .OrderByDescending(o => o.PaymentDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 設定分頁相關資訊
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = txtKeyword;

            return View(orders);
        }

        // GET: BEcpayOrders/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tEcpayOrder = await _context.TEcpayOrders
                .Include(t => t.Fposter)
                .Include(t => t.Ftask)
                .Include(t => t.Fworker)
                .FirstOrDefaultAsync(m => m.MerchantTradeNo == id);
            if (tEcpayOrder == null)
            {
                return NotFound();
            }

            return View(tEcpayOrder);
        }

        // GET: BEcpayOrders/Create
        public IActionResult Create()
        {
            ViewData["FposterId"] = new SelectList(_context.TUsers, "FuserId", "Femail");
            ViewData["FtaskId"] = new SelectList(_context.TTasks, "FtaskId", "Fdescription");
            ViewData["FworkerId"] = new SelectList(_context.TUsers, "FuserId", "Femail");
            return View();
        }

        // POST: BEcpayOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MerchantTradeNo,MemberId,RtnCode,RtnMsg,TradeNo,TradeAmt,PaymentDate,PaymentType,PaymentTypeChargeFee,TradeDate,SimulatePaid,FtaskId,FposterId,FworkerId")] TEcpayOrder tEcpayOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tEcpayOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FposterId"] = new SelectList(_context.TUsers, "FuserId", "Femail", tEcpayOrder.FposterId);
            ViewData["FtaskId"] = new SelectList(_context.TTasks, "FtaskId", "Fdescription", tEcpayOrder.FtaskId);
            ViewData["FworkerId"] = new SelectList(_context.TUsers, "FuserId", "Femail", tEcpayOrder.FworkerId);
            return View(tEcpayOrder);
        }

        // GET: BEcpayOrders/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tEcpayOrder = await _context.TEcpayOrders
                .Include(t => t.Ftask)  // 加入任務關聯
                .Include(t => t.Fposter)
                .Include(t => t.Fworker)
                .FirstOrDefaultAsync(m => m.MerchantTradeNo == id);

            if (tEcpayOrder == null)
            {
                return NotFound();
            }
            ViewData["FposterId"] = new SelectList(_context.TUsers, "FuserId", "Femail", tEcpayOrder.FposterId);
            ViewData["FtaskId"] = new SelectList(_context.TTasks, "FtaskId", "Fdescription", tEcpayOrder.FtaskId);
            ViewData["FworkerId"] = new SelectList(_context.TUsers, "FuserId", "Femail", tEcpayOrder.FworkerId);
            return View(tEcpayOrder);
        }

        // POST: BEcpayOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MerchantTradeNo,MemberId,RtnCode,RtnMsg,TradeNo,TradeAmt,PaymentDate,PaymentType,PaymentTypeChargeFee,TradeDate,SimulatePaid,FtaskId,FposterId,FworkerId")] TEcpayOrder tEcpayOrder)
        {
            if (id != tEcpayOrder.MerchantTradeNo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tEcpayOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TEcpayOrderExists(tEcpayOrder.MerchantTradeNo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FposterId"] = new SelectList(_context.TUsers, "FuserId", "Femail", tEcpayOrder.FposterId);
            ViewData["FtaskId"] = new SelectList(_context.TTasks, "FtaskId", "Fdescription", tEcpayOrder.FtaskId);
            ViewData["FworkerId"] = new SelectList(_context.TUsers, "FuserId", "Femail", tEcpayOrder.FworkerId);
            return View(tEcpayOrder);
        }

        // GET: BEcpayOrders/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tEcpayOrder = await _context.TEcpayOrders
                .Include(t => t.Fposter)
                .Include(t => t.Ftask)
                .Include(t => t.Fworker)
                .FirstOrDefaultAsync(m => m.MerchantTradeNo == id);
            if (tEcpayOrder == null)
            {
                return NotFound();
            }

            return View(tEcpayOrder);
        }

        // POST: BEcpayOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tEcpayOrder = await _context.TEcpayOrders.FindAsync(id);
            if (tEcpayOrder != null)
            {
                _context.TEcpayOrders.Remove(tEcpayOrder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TEcpayOrderExists(string id)
        {
            return _context.TEcpayOrders.Any(e => e.MerchantTradeNo == id);
        }
    }
}
