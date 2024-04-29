using otomobil.Data;
using otomobil.Models;
using otomobil.DTOs.Course;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Reflection.Metadata.BlobBuilder;
using Microsoft.AspNetCore.Authorization;
using otomobil.DTOs.User;
using otomobil.DTOs.CartItem;

namespace otomobil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CartItemController : ControllerBase
    {
        private readonly CartItemData _cartItemData;

        public CartItemController(CartItemData cartItemData)
        {
            _cartItemData = cartItemData;
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemRequestDTO cartItemDto)
        {

            if (cartItemDto == null)
                return BadRequest("Data Should be Inputed");

            CartItem cartItem = new CartItem
            {
                user_id = cartItemDto.user_id,
                course_id = cartItemDto.course_id,
                category_id = cartItemDto.category_id,
                course_name = cartItemDto.course_name,
                schedule = cartItemDto.schedule,
                price = cartItemDto.price,
                quantity = cartItemDto.quantity,
            };

            bool result = _cartItemData.Insert(cartItem);

            if (result)
            {
                return StatusCode(201, cartItem.user_id);
            }
            else
            {
                return StatusCode(500, "error occur");
            }

        }
    }

}

