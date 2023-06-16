heroku
user secrets to save api keys etc

https://codewithmukesh.com/blog/crud-with-dynamodb-in-aspnet-core/
https://codewithmukesh.com/blog/working-with-aws-s3-using-aspnet-core/#Working_with_AWS_S3_using_ASPNET_Core_%E2%80%93_Getting_Started

db conrtoller for Users
//using Amazon.DynamoDBv2.DataModel;
//using AWS.DynamoDB.Models;
//using Microsoft.AspNetCore.Mvc;

// how to get all user's playlists

string accessToken = await GetAccessToken();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            //Get the list of playlists for the user

           var playlistEndpoint = $"https://api.spotify.com/v1/users/{_userId}/playlists";
            var playlistResponse = await _client.GetAsync(playlistEndpoint);
            var content = await playlistResponse.Content.ReadAsStringAsync();

            dynamic playlistJson = JsonConvert.DeserializeObject(content);

            string result = "";
            foreach (dynamic playlist in playlistJson.items)
                result += playlist.name + ',';

//namespace DynamoStudentManager.Controllers;

//[Route("api/[controller]")]
//[ApiController]
//public class UsersController : ControllerBase
//{
//    private readonly IDynamoDBContext _context;
//    public UsersController(IDynamoDBContext context)
//    {
//        _context = context;
//    }
//    [HttpGet("{userId}")]
//    public async Task<IActionResult> GetById(int userId)
//    {
//        var user = await _context.LoadAsync<User>(userId);
//        if (user == null) return NotFound();
//        return Ok(user);
//    }
//    [HttpGet]
//    public async Task<IActionResult> GetAllUsers()
//    {
//        var user = await _context.ScanAsync<User>(default).GetRemainingAsync();
//        return Ok(user);
//    }
//    [HttpPost]
//    public async Task<IActionResult> CreateUser(User userRequest)
//    {
//        var user = await _context.LoadAsync<User>(userRequest.ID);
//        if (user != null) return BadRequest($"Student with Id {userRequest.ID} Already Exists");
//        await _context.SaveAsync(userRequest);
//        return Ok(userRequest);
//    }
//    [HttpDelete("{userId}")]
//    public async Task<IActionResult> DeleteUser(int userId)
//    {
//        var user = await _context.LoadAsync<User>(userId);
//        if (user == null) return NotFound();
//        await _context.DeleteAsync(user);
//        return NoContent();
//    }
//    [HttpPut]
//    public async Task<IActionResult> UpdateUser(User userRequest)
//    {
//        var user = await _context.LoadAsync<User>(userRequest.ID);
//        if (user == null) return NotFound();
//        //userRequest.ID = user.ID;
//        await _context.SaveAsync(userRequest);
//        return Ok(userRequest);
//    }
//}

КОРИСТУВАЧ
повинен:
- зареєструватися (юзернейм, пароль, генерується айдішнік)
- або увійти в свій профіль ввівши пароль та юзернейм
може:
- створити свою власну бібліотеку
   - завантажити свою музику в бот (перекину її в S3, там створиться бакет та папка музика)
      - вказати, назву, артиста, жанр (по бажанню) - переіменовую назву файлу в бд
      - запросити знайти під неї текст, зберегти його
   - створити свій плейлист (або декілька)
   - порпосити вивести всі пісні (кнопки)
   - попросити вивести всі плейлисти (кнопки, після нажаття вивести всі айтеми плейлисту, після нажаття вивести пісню)
   - попросити знайти кліп під пісню, чи відос в ютубчику
- зайти в споті 
   - повинен:
      - авторизуватися
   - може:
      - перевірити чи підписаний юзер на даний плейлист (по ід)
      - отримати список плейлистів карент юзера
      - отримати список плейлистів даного юзера (по ід)
      - дії з плейлистом:
         - підписатись
         - відписатьсь
         - додати/видалити айтеми
         - змінити (назву, приватність чи публічність, чи колабораторативний, дескріпшн)
         - створити
         - завантажити картинку титульну
         - отримати найкращих виконавців або композиції карент юзера
         - видалити альбоми для карент юзера
         - зберегти альбоми для карент користувача
         - показати збередені альбоми, треки карент юзера
         - 










