﻿using Microsoft.AspNetCore.Mvc;
// directive which will enable HttpClient.
using System.Net.Http;

namespace API_demo.Controllers
{
    // Tells us 
    [Route("api/[controller]")]
    [ApiController]
    public class NumbersApiController : ControllerBase
    {
        // GET: api/<NumbersApiController>
        [HttpGet]
        public string Get()
        {
            return "Welcome to the numbers API";
        }

        // GET api/<NumbersApiController>/5
        [HttpGet("{num}")]
        
        // define async method to fetch from another(numbers) API
        // Task<string> is the return type used for async methods which have return statement
        public async Task<string> Get(int num)
        {
            // define base url with input number
            string url = $"http://numbersapi.com/{num}";

            // execute using 
            try
            {
                // define http client
                using (HttpClient client = new HttpClient())
                {
                    // initiate GET request using await keyword so that it will the using statement in order
                    using(HttpResponseMessage res = await client.GetAsync(url)) 
                    {
                        if (res.IsSuccessStatusCode)
                        {
                            // get the data content from the response 
                            using (HttpContent content = res.Content)
                            {
                                // convert the content to string data and return
                                var data = await content.ReadAsStringAsync();
                                return data;
                            }
                        }
                        return res.StatusCode.ToString(); 
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

    }
}