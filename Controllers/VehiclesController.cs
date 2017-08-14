using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vega.Controllers.Resources;
using vega.Core.Models;
using vega.Core;

namespace vega.Controllers
{
    [Route("/api/vehicles")]
    public class VehiclesController : Controller
    {
        private readonly IMapper mapper;
        private readonly IVehicleRepository repository;
        private readonly IUnitOfWork unitOfWork;
        public VehiclesController(IMapper mapper, IVehicleRepository repository, IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.repository = repository;
            this.mapper = mapper;

        }
        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody] SaveVehicleResource vehicleResource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            /*
            var model = await context.Models.FindAsync(vehicleResource.ModelId);

            if (model == null)
            {
                ModelState.AddModelError("ModelId", "Invalid modelId");
                return BadRequest(ModelState);
            }
            */

            //Business rule validation
            //condition
            // if(true)
            // {
            //     //add error key-value pair
            //     ModelState.AddModelError("key", "error");
            //     //return bad request
            //     return BadRequest(ModelState);
            // }

            var vehicle = mapper.Map<SaveVehicleResource, Vehicle>(vehicleResource);
            vehicle.LastUpdate = DateTime.Now;

            //context.Vehicles.Add(vehicle);
            repository.Add(vehicle);
            //await context.SaveChangesAsync();
            await unitOfWork.CompleteAsync();

            //It's very tied with EntityFramework
            //vehicle.Model = await context.Models.Include(m => m.Make).SingleOrDefaultAsync(m => m.Id == vehicle.Model.Id);

            // vehicle = await context.Vehicles
            // .Include(v => v.Features)
            // .ThenInclude(vf => vf.Feature)
            // .Include(v => v.Model)
            // .ThenInclude(m => m.Make)
            // .SingleOrDefaultAsync(v => v.Id == vehicle.Id);
            vehicle = await repository.GetVehicle(vehicle.Id);

            var result = mapper.Map<Vehicle, VehicleResource>(vehicle);

            return Ok(result);
        }

        [HttpPut("{id}")]// /api/vehicles/{id}
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] SaveVehicleResource vehicleResource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // var model = await context.Models.FindAsync(vehicleResource.ModelId);
            // if (model == null)
            // {
            //     ModelState.AddModelError("ModelId", "Invalid modelId");
            //     return BadRequest(ModelState);
            // }

            //Business rule validation
            //condition
            // if(true)
            // {
            //     //add error key-value pair
            //     ModelState.AddModelError("key", "error");
            //     //return bad request
            //     return BadRequest(ModelState);
            // }

            //Tied with EntityFramework
            //var vehicle = await context.Vehicles.Include(v => v.Features).SingleOrDefaultAsync(v => v.Id == id);

            // var vehicle = await context.Vehicles
            // .Include(v => v.Features)
            // .ThenInclude(vf => vf.Feature)
            // .Include(v => v.Model)
            // .ThenInclude(m => m.Make)
            // .SingleOrDefaultAsync(v => v.Id == id);
            var vehicle = await repository.GetVehicle(id);

            if (vehicle == null)
                return NotFound();

            mapper.Map<SaveVehicleResource, Vehicle>(vehicleResource, vehicle);
            vehicle.LastUpdate = DateTime.Now;

            //await context.SaveChangesAsync();
            await unitOfWork.CompleteAsync();

            vehicle=await repository.GetVehicle(vehicle.Id);
            var result = mapper.Map<Vehicle, VehicleResource>(vehicle);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            //var vehicle = await context.Vehicles.FindAsync(id);
            var vehicle = await repository.GetVehicle(id, includeRelated: false);

            if (vehicle == null)
                return NotFound();

            //context.Remove(vehicle);
            repository.Remove(vehicle);
            //await context.SaveChangesAsync();
            await unitOfWork.CompleteAsync();

            return Ok(id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicle(int id)
        {
            // var vehicle = await context.Vehicles
            // .Include(v => v.Features)
            // .ThenInclude(vf => vf.Feature)
            // .Include(v => v.Model)
            // .ThenInclude(m => m.Make)
            // .SingleOrDefaultAsync(v => v.Id == id);
            var vehicle = await repository.GetVehicle(id);

            if (vehicle == null)
                return NotFound();

            var vehicleResource = mapper.Map<Vehicle, VehicleResource>(vehicle);

            return Ok(vehicleResource);
        }
    }
}