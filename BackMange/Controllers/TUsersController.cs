using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackMange.Models;

namespace BackMange.Controllers
{
    // API 控制器，處理使用者資料的 CRUD 操作
    [Route("api/[controller]")]
    [ApiController]
    public class TUsersController : ControllerBase
    {
        // 資料庫上下文
        private readonly GameCaseContext _context;

        // 建構函數
        public TUsersController(GameCaseContext context)
        {
            _context = context;
        }

        // 取得所有使用者列表
        // GET: api/TUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TUser>>> GetTUsers()
        {
            return await _context.TUsers.ToListAsync();
        }

        // 取得指定 ID 的使用者
        // GET: api/TUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TUser>> GetTUser(int id)
        {
            var tUser = await _context.TUsers.FindAsync(id);

            if (tUser == null)
            {
                return NotFound();
            }

            return tUser;
        }

        // 更新使用者資料
        // PUT: api/TUsers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTUser(int id, TUser tUser)
        {
            if (id != tUser.FuserId)
            {
                return BadRequest();
            }

            _context.Entry(tUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // 新增使用者
        // POST: api/TUsers
        [HttpPost]
        public async Task<ActionResult<TUser>> PostTUser(TUser tUser)
        {
            _context.TUsers.Add(tUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTUser", new { id = tUser.FuserId }, tUser);
        }

        // 刪除使用者
        // DELETE: api/TUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTUser(int id)
        {
            var tUser = await _context.TUsers.FindAsync(id);
            if (tUser == null)
            {
                return NotFound();
            }

            _context.TUsers.Remove(tUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 檢查使用者是否存在
        private bool TUserExists(int id)
        {
            return _context.TUsers.Any(e => e.FuserId == id);
        }
    }
}