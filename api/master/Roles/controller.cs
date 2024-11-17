

// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;

// namespace Goarif.Server.Controllers
// {
//     [Authorize]
//     [ApiController]
//     [Route("/[controller]")]
//     public class RoleController : ControllerBase
//     {
//         private readonly IRoleService _IRoleService;
//         private readonly ErrorHandlingUtility _errorUtility;
//         private readonly ValidationMasterDto _masterValidationService;
//         public RoleController(IRoleService roleService)
//         {
//             _IRoleService = roleService;
//             _errorUtility = new ErrorHandlingUtility();
//             _masterValidationService = new ValidationMasterDto();
//         }

//         // [Authorize]
//         [HttpGet]
//         public async Task<object> Get()
//         {
//             try
//             {
//                 var data = await _IRoleService.Get();
//                 return Ok(data);
//             }
//             catch (CustomException ex)
//             {
//                 int errorCode = ex.ErrorCode;
//                 var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
//                 return _errorUtility.HandleError(errorCode, errorResponse);
//             }
//         }

//         [HttpGet("{id}")]
//         public async Task<object> GetById([FromRoute] string id)
//         {
//             try
//             {
//                 var data = await _IRoleService.GetById(id);
//                 return Ok(data);
//             }
//             catch (CustomException ex)
//             {
//                 int errorCode = ex.ErrorCode;
//                 var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
//                 return _errorUtility.HandleError(errorCode, errorResponse);
//             }
//         }

//         // [Authorize]
//         [HttpPost]
//         public async Task<object> Post([FromBody] CreateRoleDto item)
//         {
//             try
//             {
//                 var validationErrors = _masterValidationService.ValidateCreateInput(item);
//                 if (validationErrors.Count > 0)
//                 {
//                     var errorResponse = new { code = 400, errorMessage = validationErrors };
//                     return BadRequest(errorResponse);
//                 }
//                 var data = await _IRoleService.Post(item);
//                 return Ok(data);
//             }
//             catch (CustomException ex)
//             {
//                 int errorCode = ex.ErrorCode;
//                 var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
//                 return _errorUtility.HandleError(errorCode, errorResponse);
//             }
//         }

//         // [Authorize]
//         [HttpPut("{id}")]
//         public async Task<object> Put([FromRoute] string id, [FromBody] CreateRoleDto item)
//         {
//             try
//             {
//                 var validationErrors = _masterValidationService.ValidateCreateInput(item);
//                 if (validationErrors.Count > 0)
//                 {
//                     var errorResponse = new { code = 400, errorMessage = validationErrors };
//                     return BadRequest(errorResponse);
//                 }
//                 var data = await _IRoleService.Put(id, item);
//                 return Ok(data);
//             }
//             catch (CustomException ex)
//             {
//                 int errorCode = ex.ErrorCode;
//                 var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
//                 return _errorUtility.HandleError(errorCode, errorResponse);
//             }
//         }

//         // [Authorize]
//         [HttpDelete("{id}")]
//         public async Task<object> Delete([FromRoute] string id)
//         {
//             try
//             {
//                 var data = await _IRoleService.Delete(id);
//                 return Ok(data);
//             }
//             catch (CustomException ex)
//             {
//                 int errorCode = ex.ErrorCode;
//                 var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
//                 return _errorUtility.HandleError(errorCode, errorResponse);
//             }
//         }


//     }
// }