using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using System.Threading;
using Newtonsoft.Json;

namespace AzureSearchTest
{
    public class Accessory
    {
        public string id { get; set; }
        public string recordSetTotal { get; set; }
        public string resourceId { get; set; }
        public string resourceName { get; set; }
        public string recordSetStartNumber { get; set; }
        public string recordSetComplete { get; set; }
        public ICollection<string> FacetView { get; set; }
        public ICollection<string> CatalogEntryView { get; set; }
        public string recordSetCount { get; set; }
        public ICollection<string> MetaData { get; set; }
    }

    public class AccesoryItem
    {
        public string longDescription { get; set; }
        public string buyable { get; set; }
        public string shortDescription { get; set; }
        public string parentCategoryID { get; set; }
        public ICollection<string> Price { get; set; }
        public string productType { get; set; }
        public string resourceId { get; set; }
        public string name { get; set; }
        public string partNumber { get; set; }
        public string uniqueID { get; set; }
        public string storeID { get; set; }
    }
    public class Hotel
    {
        public string HotelId { get; set; }

        public string HotelName { get; set; }

        public double? BaseRate { get; set; }

        public string Category { get; set; }

        public string[] Tags { get; set; }

        public bool? ParkingIncluded { get; set; }

        public DateTimeOffset? LastRenovationDate { get; set; }

        public int? Rating { get; set; }

        public GeographyPoint Location { get; set; }

        public override string ToString()
        {
            return String.Format(
                "ID: {0}\tName: {1}\tCategory: {2}\tTags: [{3}]",
                HotelId,
                HotelName,
                Category,
                (Tags != null) ? String.Join(", ", Tags) : String.Empty);
        }
    }
    class car
    {
        public string id { get; set; }
        public string Description { get; set; }
        public string Model { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string searchServiceName = "dcsearch";

            string apiKey = "9EE9884EDF41DBF7EED8C4B74E8ABBDE";


            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            SearchIndexClient indexClient = serviceClient.Indexes.GetClient("accesoriesindex");

            //Console.WriteLine("{0}", "Uploading documents...\n");
            //UploadDocuments(indexClient);

            Console.WriteLine("{0}", "Searching documents 'V40'...\n");
            SearchDocuments(indexClient, searchText: "32767fdb-b4e7-f1b5-d5c4-8507c7942adc");

            //Console.WriteLine("\n{0}", "Filter documents with Description 'blabla'...\n");
            //SearchDocuments(indexClient, searchText: "*", filter: "Description eq 'blabla'");
        }
        private static void SearchDocuments(SearchIndexClient indexClient, string searchText, string filter = null)
        {
            // Execute search based on search text and optional filter
            var sp = new SearchParameters();

            if (!String.IsNullOrEmpty(filter))
            {
                sp.Filter = filter;
            }

            DocumentSearchResult<car> response = indexClient.Documents.Search<car>(searchText, sp);
            DocumentSearchResult<Accessory> res = indexClient.Documents.Search<Accessory>(searchText, sp);


            DocumentSearchResult<Accessory> result2 = indexClient.Documents.Search<Accessory>(searchText, sp);
            DocumentSearchResult searchResult = indexClient.Documents.Search(searchText, sp);
            foreach (SearchResult<car> result in response.Results)
            {
                Console.WriteLine("id = " + result.Document.id);
                Console.WriteLine("Model = " + result.Document.Model);
                    Console.WriteLine("Description = " + result.Document.Description);
            }
            Console.ReadLine();

            foreach (SearchResult<Accessory> accRes in res.Results)
            {
                Console.WriteLine(accRes.Document.CatalogEntryView);
                //var results = JsonConvert.DeserializeObject<AccesoryItem>(accRes.Document.CatalogEntryView.ElementAt(0));

                foreach (string accItem in accRes.Document.CatalogEntryView)
                {
                    Console.WriteLine(accItem);
                }
                //Console.WriteLine("id = " + accRes.Document.name);
                //Console.WriteLine("Model = " + accRes.Document.Model);
                //Console.WriteLine("Description = " + accRes.Document.Description);
            }
            Console.ReadLine();

        }

        private static void UploadDocuments(SearchIndexClient indexClient)
        {
            var documents =
                new AccesoryItem[]
                {
            new AccesoryItem()
            {
               
            }
                };

            try
            {
                var batch = IndexBatch.Upload(documents);
                indexClient.Documents.Index(batch);
            }
            catch (IndexBatchException e)
            {
                // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                // the batch. Depending on your application, you can take compensating actions like delaying and
                // retrying. For this simple demo, we just log the failed document keys and continue.
                Console.WriteLine(
                    "Failed to index some of the documents: {0}",
                    String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
            }

            // Wait a while for indexing to complete.
            Thread.Sleep(2000);
        }   
    }
}
