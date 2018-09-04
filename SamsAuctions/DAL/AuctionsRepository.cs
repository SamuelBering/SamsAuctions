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
    public class AuctionsRepository : IAuctionsRepository
    {
        const string baseUrl = "http://nackowskis.azurewebsites.net";

        public async Task AddOrUpdateAuction(Auction auction)
        {
            if (auction.AuktionID > 0)
                await Put("auktion", auction);
            else
                await Post("auktion", auction);
        }

        public async Task<IList<Auction>> GetAllAuctions(int groupCode)
        {

            DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new System.Runtime.Serialization.DateTimeFormat("yyyy-MM-dd'T'HH:mm:ss")
            };

            var auctionList = await Get<List<Auction>>($"auktion/{groupCode}", settings);

            return auctionList;

        }

        public async Task<IList<Bid>> GetAllBids(int groupCode, int auctionId)
        {

            var bidsList = await Get<List<Bid>>($"bud/{groupCode}/{auctionId}");

            return bidsList;

        }

        public async Task RemoveAuction(int auctionId, int groupCode)
        {
            await Delete($"{groupCode}/{auctionId}");
        }

        private void SetupRequest(HttpClient client)
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new
            MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task<T> Get<T>(string query, DataContractJsonSerializerSettings settings = null)
        {
            using (HttpClient client = new HttpClient())
            {

                SetupRequest(client);
                HttpResponseMessage response =
                       await client.GetAsync($"/api/{query}");
                response.EnsureSuccessStatusCode();
                DataContractJsonSerializer serializer;

                if (settings != null)
                    serializer = new DataContractJsonSerializer(typeof(T), settings);
                else
                    serializer = new DataContractJsonSerializer(typeof(T));

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
                       await client.PostAsync($"/api/{query}", new JsonContent(model));
                response.EnsureSuccessStatusCode();
            }
        }

        private async Task Put<T>(string query, T model)
        {
            using (HttpClient client = new HttpClient())
            {

                SetupRequest(client);
                HttpResponseMessage response =
                       await client.PutAsync($"/api/{query}", new JsonContent(model));
                response.EnsureSuccessStatusCode();
            }
        }

        private async Task Delete(string query)
        {
            using (HttpClient client = new HttpClient())
            {

                SetupRequest(client);
                HttpResponseMessage response =
                       await client.DeleteAsync($"/api/auktion/{query}");
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task<Auction> GetAuction(int id, int groupCode)
        {
            DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new System.Runtime.Serialization.DateTimeFormat("yyyy-MM-dd'T'HH:mm:ss")
            };

            var auction = await Get<Auction>($"auktion/{groupCode}?id={id}", settings);

            return auction;
        }

        public async Task AddBid(Bid bid)
        {
            await Post("bud", bid);
        }
    }
}
