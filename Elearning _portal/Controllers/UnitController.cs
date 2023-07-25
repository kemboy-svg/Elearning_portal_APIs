using Elearning__portal.Data;
using Elearning__portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Elearning__portal.Controllers
{
    public class UnitController : Controller
    {
        private readonly DtabaseSet _dtabaseSet;

        public UnitController(DtabaseSet dtabaseSet)
        {
            _dtabaseSet = dtabaseSet;
        }
        [HttpPost]
        [Route("api/Unit")]
        public async Task<IActionResult> Add([FromBody] Unit model )
        {

            try
            {
                var unit = new Unit
                {
                    Id = model.Id,
                    unit_code = model.unit_code,
                    unit_name = model.unit_name
                };
                await _dtabaseSet.Units.AddAsync(unit);
                await _dtabaseSet.SaveChangesAsync();
                return StatusCode(200, "Added Successfully");

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while adding the unit: " + ex.Message);
            }
            }


        [HttpPut]
        [Route("api/Unit{id}")]
        public async Task<IActionResult> Update( [FromBody] Unit model, int id )
        {
            try
            {
                var unit = await _dtabaseSet.Units.FindAsync(id);

                if (unit == null)
                {
                    return NotFound("Unit not found");
                }

                // Update the unit properties
                unit.unit_code = model.unit_code;
                unit.unit_name = model.unit_name;

                _dtabaseSet.Units.Update(unit);
                await _dtabaseSet.SaveChangesAsync();

                return StatusCode(200, "Updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the unit: " + ex.Message);
            }
        }

        [HttpDelete]
        [Route("api/Unit/Delete{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var unit = await _dtabaseSet.Units.FindAsync(id);

                if (unit == null)
                {
                    return NotFound("Unit not found");
                }

                _dtabaseSet.Units.Remove(unit);
                await _dtabaseSet.SaveChangesAsync();

                return StatusCode(200, "Deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the unit: " + ex.Message);
            }
        }


        [HttpGet]
        [Route("api/Units")]
        public async Task<IActionResult> Get()
        {
            var units  = await _dtabaseSet.Units.ToListAsync();
            return Ok(units);
        }
    }
}
