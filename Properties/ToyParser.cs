using AngleSharp;
using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Properties
{
    internal class ToyParser
    {
        
        // Нахождение количества страниц
        public static async Task<int> GetPagination(string url, BrowsingContext context)
        {
            
            var document = await context.OpenAsync(url);
            string pageSelector = "#wrapper > main > div > div > div.col-12.col-sm-12.col-md-12.col-lg-9 > noindex:nth-child(7) > nav > ul > li:nth-child(8) > a";
            string pagesCount = document.QuerySelector(pageSelector).TextContent;
            int pagination = int.Parse(pagesCount);
            return pagination;
        }

        // Нахождение ссылок на все игрушки страницы 
        public static async Task<List<string>> GetToyHrefs(string url, BrowsingContext context)
        {
            // Здесь в документ передается html файл вместо ссылки на страницу (чтобы уменьшить количество запросов)
            var document = await context.OpenAsync(url);

            // Вернет список тегов "a" с классом "text-center"
            var toyTags = document.All.Where(m => m.LocalName == "a" && m.ClassList.Contains("text-center"));

            // Создать список и итерируясь по нему в цикле взять атрибут "href"
            var hrefList = new List<string>();
            foreach (var tag in toyTags)
            {
                hrefList.Add($"https://www.toy.ru" + tag.GetAttribute("href"));
            }

            return hrefList;
        }

        // Метод для упрощения работы с классом
        public static async Task<string> GetItemByClass(string url, string tagName, string className)
        {
            try
            {
                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);
                var document = await context.OpenAsync(url);
                var itemTag = document.All.Where(m => m.LocalName == tagName && m.ClassList.Contains(className));
                string item = string.Empty;
                foreach (var tag in itemTag)
                {
                    string priceText = tag.TextContent;
                    item += priceText;
                }
                return item;

            }
            catch { return "   -   "; }

        }

        // Находим данные региона по селектору
        public static async Task<string> GetRegion(string url) {

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(url);
            string regionSelector = "#wrapper > header > div.d-none.d-md-block.top-line > div > div > div.col-6.col-lg-3.justify-content-start > div > noindex > div.container > div > div > a";
            try
            {
                string region = document.QuerySelector(regionSelector).TextContent;
                string reg = region.Trim();
                return reg;
            }
            catch { return "   -   "; }
        }

        // Название товара
        public static async Task<string> GetToyName(string url)
        {
            string toyName = await GetItemByClass(url, "h1", "detail-name");
            toyName = toyName.Replace(",", string.Empty);
            return toyName;
        }

        // Цена
        public static async Task<string> GetPrice(string url) 
        {
            string price = await GetItemByClass(url, "span", "price");
            price = price.Replace(",", string.Empty);
            return price;
           
        }

        // Старая цена
        public static async Task<string> GetOldPrice(string url) 
        {
            string oldPrice = await GetItemByClass(url, "span", "old-price");
            oldPrice = oldPrice.Replace(",", string.Empty);
            return oldPrice;
         

        }

        // Раздел с наличием
        public static async Task<string> IsInStok(string url)
        {
            string inStoke = await GetItemByClass(url, "span", "ok");
            return inStoke;
        
        }

        // Хлебные крошки
        public static async Task<string> GetBreadCrumb(string url, BrowsingContext context)
        {
            try
            {
                var document = await context.OpenAsync(url);
                var breadCrumb = document.All.Where(m => m.LocalName == "a" && m.ClassList.Contains("breadcrumb-item"));

                string breadCrumbs = string.Empty;
                foreach (var tag in breadCrumb)
                {
                    string crumb = tag.GetAttribute("title") + " > ";
                    breadCrumbs += crumb;
                }

                string lastCrumbSelector = "#wrapper > main > div > div > div:nth-child(1) > nav > span.breadcrumb-item.active.d-none.d-block";
                string lastCrumb = document.QuerySelector(lastCrumbSelector).TextContent;

                breadCrumbs += lastCrumb;
                breadCrumbs = breadCrumbs.Replace(",", string.Empty);
                return breadCrumbs;

            }catch { return "   -   "; }
        }

        // Картинки
        public static async Task<List<string>> GetPictures(string url, BrowsingContext context) {
            try
            {
                var document = await context.OpenAsync(url);
                var pictures = document.All.Where(m => m.LocalName == "img" && m.ClassList.Contains("img-fluid"));
                var picturesList = new List<string>();

                foreach (var tag in pictures)
                {
                    picturesList.Add(tag.GetAttribute("src"));
                }
                picturesList.RemoveAt(0);
                picturesList.RemoveAt(0);
                picturesList.RemoveAt(picturesList.Count - 1);

                return picturesList;

            }catch { return null; }
        }


    }
}
