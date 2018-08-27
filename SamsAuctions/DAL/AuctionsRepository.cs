using Newtonsoft.Json;
using SamsAuctions.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using SamsAuctions.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using SamsAuctions.Infrastructure;

namespace SamsAuctions.DAL
{
    [Authorize]
    public class AuctionsRepository: IAuctionsRepository
    {
        const string baseUrl = "http://nackowskis.azurewebsites.net";

        public async Task AddOrUpdateAuction(AuctionViewModel viewModel)
        {
            var auction = Mapper.Map<AuctionViewModel, Auction>(viewModel);

            await Post("auktion", auction);

        }

        private void SetupRequest(HttpClient client)
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new
            MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task<T> Get<T>(string query)
        {
            using (HttpClient client = new HttpClient())
            {

                SetupRequest(client);
                HttpResponseMessage response =
                       await client.GetAsync($"/api/{query}");
                response.EnsureSuccessStatusCode();             
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T)); 
                Stream responseStream = await response.Content.ReadAsStreamAsync();              
                T data = (T)serializer.ReadObject(responseStream);
                var answer = await response.Content.ReadAsStringAsync();
                return data;
            }
        }

        private async Task Post<T>(string query, T model)
        {
            using (HttpClient client = new HttpClient())
            {

                SetupRequest(client);
                HttpResponseMessage response =
                       await client.PostAsync($"/api/{query}",new JsonContent(model));
                response.EnsureSuccessStatusCode();
            }
        }
       

    }
}
