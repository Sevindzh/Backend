using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly BuildingContext _context;

        public UsersController(BuildingContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("{id}")] //localhost:5000/api/Users/2
        public async Task<ActionResult> GetUsersById(int id)
        {
            return Ok(await _context.Users.FindAsync(id));
        }

        [HttpGet("ByName/{name}")]  //localhost:5000/api/Users/ByName/Boris Britva
        public async Task<ActionResult> GetUsersByName(string name)
        {
            var projects = _context.DesignProjects
                .Include(p => p.Apt)
                .ThenInclude(a => a.Bldng)
                .Include(p => p.Usr)
                .Where(p => p.Usr.UName == name);
            string Result = "";
            foreach (DesignProject pr in projects)
                Result = Result + pr.ToString() + "\n";
            return Ok(Result);
        }

        [HttpGet("GetProjectsForRoomN/{n}/{name}")]
        public async Task<ActionResult> GetProjectsForRoomN(int n, string name)
        {
            //var apartments = await _context.Apartments
            //    .Include(a => a.ProjectsForApt)
            //    .ThenInclude(p => p.Usr).Where(a => a.RoomsN == n);
            var projects = _context.DesignProjects
                .Include(p => p.Apt)
                .ThenInclude(a => a.Bldng)
                .Where(p => p.Apt.RoomsN == n)
                .Include(p => p.Usr)
                .Where(p => p.Usr.UName == name);
            string Result = "";
            foreach (DesignProject pr in projects)
                Result = Result + pr.ToString()+"\n";
            return Ok(Result);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (user == null)
            {
                return NotFound();
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsers", user);
        }

        [HttpPost("Project"), Authorize(Roles = "User")]
        public async Task<ActionResult<DesignProject>> PostProject(DesignProject project)
        {
            var IdClaim = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            if (IdClaim == project.UserID)
            {
                if (project == null)
                {
                    return NotFound();
                }
                _context.DesignProjects.Add(project);
                await _context.SaveChangesAsync();
                return project;
            }
            else
                return BadRequest("Error");
        }

        [HttpPut("{id}"), Authorize(Roles = "User")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            var IdClaim = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (id != IdClaim)
            {
                return BadRequest("Error");
            }
            else
            {
                User ExistingUser = _context.Users.FindAsync(IdClaim).Result;
                if (user.UName != null)
                    ExistingUser.UName = user.UName;
                if (user.USurame != null)
                    ExistingUser.USurame = user.USurame;

                _context.Entry(ExistingUser).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return Ok(user);
            }
        }

        [HttpPut("Project/{id}"), Authorize(Roles = "User")]
        public async Task<IActionResult> PutProject(int id, DesignProject project)
        {
            var IdClaim = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (project.DesignProjectID != id || project.UserID != IdClaim)
            {
                return BadRequest("Error");
            }
            else
            {
                DesignProject ExistinProject = _context.DesignProjects.FindAsync(id).Result;
                if (project.AptID != ExistinProject.DesignProjectID)
                    ExistinProject.AptID = project.AptID;
                if (project.Photo != null)
                    ExistinProject.Photo = project.Photo;

                _context.Entry(ExistinProject).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.DesignProjectID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return Ok(_context.DesignProjects
                    .Include(p => p.Usr)
                    .Where(p => p.DesignProjectID ==project.DesignProjectID));
            }
        }

        [HttpDelete("{id}"), Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var RoleClaim = User.FindFirstValue(ClaimTypes.Role);
            var IdClaim = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (IdClaim == id || RoleClaim == "Admin")
            {
                var user = await _context.Users.Include(u => u.Projects).FirstOrDefaultAsync(u => u.UserID == id);
                if (user == null)
                {
                    return NotFound();
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return user;
            }
            else
                return BadRequest("Error");
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }

        private bool ProjectExists(int id)
        {
            return _context.DesignProjects.Any(e => e.DesignProjectID == id);
        }

    }
}
