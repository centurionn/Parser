using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Text;
using Parser.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Настройка конфигурации
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            // Путь до CSV файла
            string dataFile = @"C:\Users\rybya\Desktop\Data3.csv";
            var csv = new StringBuilder();

            // Количество страниц
            int pagination = await ToyParser.GetPagination("https://www.toy.ru/catalog/boy_transport/?filterseccode%5B0%5D=transport", (BrowsingContext) context);

            
            // Цикл по страницам
            for (int i = 1; i <= pagination; i++)
            {
                // URL с номером страницы
                string url = $"https://www.toy.ru/catalog/boy_transport/?filterseccode%5B0%5D=transport&PAGEN_8={i}";

                List<string> toyLinkList = await ToyParser.GetToyHrefs(url, (BrowsingContext)context); // Список ссылок на игрушки данной страницы

                // Цикл по ссылкам
                int count = 1;

                foreach (var link in toyLinkList)
                {
                    string region = string.Empty;
                    await Task.Run(async () =>
                    {
                        region = await ToyParser.GetRegion(link); // Регион
                    });

                    string toyName = string.Empty;
                    await Task.Run(async () =>
                    {
                        toyName = await ToyParser.GetToyName(link); // Название игрушки
                    });

                    string breadCrumbs = string.Empty;
                    await Task.Run(async () =>
                    {
                        breadCrumbs = await ToyParser.GetBreadCrumb(link, (BrowsingContext)context);// Хлебные крошки
                    });

                    string price = string.Empty;
                    await Task.Run(async () =>
                    {
                        price = await ToyParser.GetPrice(link); // Новая цена
                    });

                    string oldPrice = string.Empty;
                    await Task.Run(async () =>
                    {
                        oldPrice = await ToyParser.GetOldPrice(link); // Старая цена
                    });

                    string isInStok = string.Empty;
                    await Task.Run(async () =>
                    {
                        isInStok = await ToyParser.IsInStok(link); // В наличии
                    });

                    string images = string.Empty;
                    await Task.Run(async () =>
                    {
                        List<string> list = await ToyParser.GetPictures(link, (BrowsingContext)context); // Картинки
                        images = String.Join("  >  ", list.Select(x => x.ToString()).ToArray());
                    });
                              
                    // Формирование csv ряда
                    var newLine = $"{link}, {region}, {toyName}, {breadCrumbs}, {price}, {oldPrice}, {isInStok}, {images}";
                    csv.AppendLine(newLine);
                   
                    Console.WriteLine($"Страница:{i}");
                    Console.WriteLine($"    Индекс текущего товара:{count}");
                    count++;
                }

            }         
            // Запись в csv
            File.WriteAllText(dataFile, csv.ToString(), Encoding.UTF8);
            Console.ReadKey();
        }
    }
}


