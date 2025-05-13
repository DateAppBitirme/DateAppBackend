using DateApp.Data;
using DateApp.Dtos.ComplaintRequestDto;
using DateApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static DateApp.Core.Enums.ComplaintRequest;

namespace DateApp.Controllers
{
    public class ComplaintRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ComplaintRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllComplaintAndRequests")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var complaintsAndRequests = await _context.ComplaintAndRequests
                .Include(c => c.User)
                .Where(c => !c.IsDeleted)
                .Select(c => new ComplaintAndRequestDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    IsActive = c.IsActive,
                    UserName = c.User != null ? c.User.UserName : ""
                })
                    .ToListAsync();

                return Ok(complaintsAndRequests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("GetAllComplaint")]
        public async Task<IActionResult> GetAllComplaint()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var complaints = await _context.ComplaintAndRequests
                    .Include(c => c.User)
                    .Where(c => c.IsActive && !c.IsDeleted && c.ComplaintTypeId != null)
                    .Select(c => new ComplaintAndRequestDto
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Description = c.Description,
                        CreatedAt = c.CreatedAt,
                        IsActive = c.IsActive,
                        UserName = c.User != null ? c.User.UserName : ""
                    })
                    .ToListAsync();
                return Ok(complaints);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetAllRequest")]
        public async Task<IActionResult> GetAllRequest()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var requests = await _context.ComplaintAndRequests
                .Include(c => c.User)
                .Where(c => c.IsActive && !c.IsDeleted && c.RequestTypeId != null)
                .Select(c => new ComplaintAndRequestDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    IsActive = c.IsActive,
                    UserName = c.User != null ? c.User.UserName : ""
                })
                    .ToListAsync();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteComplaintAndRequest/{id}")]
        public async Task<IActionResult> DeleteComplaintAndRequest(int id) //Soft delete yaptım. Yani veriler silinse de görünebilecek.
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var complaintAndRequest = await _context.ComplaintAndRequests.FindAsync(id);
                if (complaintAndRequest == null)
                {
                    return NotFound("Şikayet veya istek bulunamadı.");
                }
                complaintAndRequest.IsDeleted = true;
                _context.ComplaintAndRequests.Update(complaintAndRequest);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Şikayet veya istek başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("ResolveComplaintAndRequest/{id}")]
        public async Task<IActionResult> ResolveComplaintAndRequest(int id) 
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var complaintAndRequest = await _context.ComplaintAndRequests.FindAsync(id);
                if (complaintAndRequest == null)
                {
                    return NotFound("Şikayet veya istek bulunamadı.");
                }
                complaintAndRequest.IsActive = false;
                _context.ComplaintAndRequests.Update(complaintAndRequest);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Şikayet veya istek başarıyla çözüldü." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("CreateComplaint")]
        public async Task<IActionResult> CreateComplaintOrRequest([FromBody] CreateComplaintDto complaintDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!Enum.IsDefined(typeof(ComplaintType), complaintDto.ComplaintTypeId!))
                {
                    return BadRequest("Geçersiz istek türü.");
                }

                var complaint = new ComplaintAndRequest
                {
                    Title = complaintDto.Title,
                    Description = complaintDto.Description,
                    CreatedAt = DateTime.UtcNow,
                    ComplaintTypeId = complaintDto.ComplaintTypeId,
                    RequestTypeId = null,
                    IsActive = true,
                    IsDeleted = false,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };
                
                await _context.ComplaintAndRequests.AddAsync(complaint);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Şikayet başarıyla oluşturuldu." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("CreateRequest")]
        public async Task<IActionResult> CreateRequest([FromBody] CreateRequestDto requestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!Enum.IsDefined(typeof(RequestType), requestDto.RequestTypeId!))
                {
                    return BadRequest("Geçersiz istek türü.");
                }

                var request = new ComplaintAndRequest
                {
                    Title = requestDto.Title,
                    Description = requestDto.Description,
                    CreatedAt = DateTime.UtcNow,
                    RequestTypeId = requestDto.RequestTypeId,
                    ComplaintTypeId = null,
                    IsActive = true,
                    IsDeleted = false,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };
                
                await _context.ComplaintAndRequests.AddAsync(request);
                await _context.SaveChangesAsync();
                return Ok(new { message = "İstek başarıyla oluşturuldu." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
    

    
}