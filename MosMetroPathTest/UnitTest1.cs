using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MosMetroPath;
using System.Linq;
using System.Diagnostics;

namespace MosMetroPathTest
{
    [TestClass]
    public class UnitTest1
    {
        private Scheme Scheme;

        public UnitTest1()
        {
            Scheme = new Scheme();

            var red = Scheme.AddLine(@"Сокольническая");
            var green = Scheme.AddLine(@"Замоскворецкая");
            var gray = Scheme.AddLine(@"Серпуховско-Тимирязевская");
            var ring = Scheme.AddLine(@"Кольцевая");
            var fili = Scheme.AddLine(@"Филёвская");
            var blue = Scheme.AddLine(@"Арбатско-Покровская");
            var butovo = Scheme.AddLine(@"Бутовская");
            var kahovskaya = Scheme.AddLine(@"Каховская");
            var lublino_dmitr = Scheme.AddLine(@"Люблинско-Дмитровская");

            // Серпуховско-Тимирязевская
            gray
                .AddStation(@"Бульвар Дмитрия Донского", out var don)
                .RelationTo(@"Аннино", 140)
                .RelationTo(@"Улица Академика Янгеля", 100)
                .RelationTo(@"Пражская", 130)
                .RelationTo(@"Южная", 90)
                .RelationTo(@"Чертановская", 170)
                .RelationTo(@"Севастопольская", 120, out var sevastopol)
                .RelationTo(@"Нахимовский проспект", 90)
                .RelationTo(@"Нагорная", 90)
                .RelationTo(@"Нагатинская", 100)
                .RelationTo(@"Тульская", 220)
                .RelationTo(@"Серпуховская", 180, out var serpuh)
                .RelationTo(@"Полянка", 100)
                .RelationTo(@"Боровицкая", 130, out var borov)
                .RelationTo(@"Чеховская", 130, out var chehov)
                .RelationTo(@"Цветной бульвар", 90, out var tzhvetnoy)
                .RelationTo(@"Менделеевская", 140, out var mendeleev)
                .RelationTo(@"Савёловская", 100, out var savel)
                .RelationTo(@"Дмитровская", 110)
                .RelationTo(@"Тимирязевская", 80, out var timir)
                .RelationTo(@"Петровско-Разумовская", 170, out var petrovsko)
                .RelationTo(@"Владыкино", 140, out var vladik)
                .RelationTo(@"Отрадное", 150)
                .RelationTo(@"Бибирево", 170)
                .RelationTo(@"Алтуфьево", 160);

            // Замоскворецкая
            green
                .AddStation(@"Алма-Атинская")
                .RelationTo(@"Красногвардейская", 360, out var krasnogvard)
                .RelationTo(@"Домодедовская", 130)
                .RelationTo(@"Орехово", 130)
                .RelationTo(@"Царицыно", 150)
                .RelationTo(@"Кантемировская", 130)
                .RelationTo(@"Каширская", 180, out var kashir)
                .RelationTo(@"Коломенская", 210)
                .RelationTo(@"Технопарк", 180)
                .RelationTo(@"Автозаводская", 180, out var avtozavod)
                .RelationTo(@"Павелецкая", 190, out var pavel_green)
                .RelationTo(@"Новокузнецкая", 120)
                .RelationTo(@"Театральная", 140, out var teatral)
                .RelationTo(@"Тверская", 100, out var tver)
                .RelationTo(@"Маяковская", 80)
                .RelationTo(@"Белорусская", 100, out var belorus_green)
                .RelationTo(@"Динамо", 160, out var dinamo)
                .RelationTo(@"Аэропорт", 160)
                .RelationTo(@"Сокол", 110)
                .RelationTo(@"Войковская", 150, out var voykov)
                .RelationTo(@"Водный стадион", 180)
                .RelationTo(@"Речной вокзал", 140)
                .RelationTo(@"Ховрино", 300);

            // Сокольническая
            red
                .AddStation(@"Бульвар Рокоссовского")
                .RelationTo(@"Черкизовская", 130, out var cherkiz)
                .RelationTo(@"Преображенская площадь", 200)
                .RelationTo(@"Сокольники", 170)
                .RelationTo(@"Красносельская", 100)
                .RelationTo(@"Комсомольская", 80, out var komsomol_red)
                .RelationTo(@"Красные ворота", 90)
                .RelationTo(@"Чистые пруды", 80, out var prudi)
                .RelationTo(@"Лубянка", 90, out var lubyanka)
                .RelationTo(@"Охотный ряд", 80, out var ohotniy)
                .RelationTo(@"Библиотека им. Ленина", 110, out var biblioteka)
                .RelationTo(@"Кропоткинская", 80)
                .RelationTo(@"Парк культуры", 100, out var park_kult_red)
                .RelationTo(@"Фрунзенская", 120)
                .RelationTo(@"Спортивная", 80)
                .RelationTo(@"Воробьёвы горы", 150)
                .RelationTo(@"Университет", 180)
                .RelationTo(@"Проспект Вернадского", 150)
                .RelationTo(@"Юго-Западная", 150)
                .RelationTo(@"Тропарёво", 180)
                .RelationTo(@"Румянцево", 180)
                .RelationTo(@"Саларьево", 180);
            // Кольцевая
            ring
                .AddStation(@"Парк культуры", out var park_kul_ring)
                .RelationTo(@"Октябрьская", 100, out var oktyabr)
                .RelationTo(@"Добрынинская", 90, out var dobrin)
                .RelationTo(@"Павелецкая", 90, out var pavel_ring)
                .RelationTo(@"Таганская", 110, out var tagan_ring)
                .RelationTo(@"Курская", 140, out var kursk_ring)
                .RelationTo(@"Комсомольская", 160, out var komsomol_ring)
                .RelationTo(@"Проспект Мира", 140)
                .RelationTo(@"Новослободская", 130, out var novoslob)
                .RelationTo(@"Белорусская", 110, out var belorus_ring)
                .RelationTo(@"Краснопресненская", 140)
                .RelationTo(@"Киевская", 150, out var kiev_ring);
            // Филёвская
            fili
                .AddStation(@"Кунцевская", out var kunzevo_fili)
                .RelationTo(@"Пионерская", 110)
                .RelationTo(@"Филевский парк", 100)
                .RelationTo(@"Багратионовская", 100)
                .RelationTo(@"Фили", 140)
                .RelationTo(@"Кутузовская", 140)
                .RelationTo(@"Студенческая", 100)
                .RelationTo(@"Киевская", 120, out var kiev_fili)
                .RelationTo(@"Смоленская", 130)
                .RelationTo(@"Арбатская", 100)
                .RelationTo(@"Александровский сад", 70, out var aleks_sad);
            kiev_fili
                .RelationTo(@"Выставочная", 300, out var vistav)
                .RelationTo(@"Международная", 120, out var mezhdunarod);

            // Арбатско-Покровская
            blue
                .AddStation(@"Щёлковская")
                .RelationTo(@"Первомайская", 130)
                .RelationTo(@"Измайловская", 180)
                .RelationTo(@"Партизанская", 180, out var partizan)
                .RelationTo(@"Семёновская", 150)
                .RelationTo(@"Электрозаводская", 110)
                .RelationTo(@"Бауманская", 140)
                .RelationTo(@"Курская", 170, out var kursk_blue)
                .RelationTo(@"Площадь Революции", 170, out var revol)
                .RelationTo(@"Арбатская", 130, out var arbat_blue)
                .RelationTo(@"Смоленская", 140)
                .RelationTo(@"Киевская", 100, out var kiev_blue)
                .RelationTo(@"Парк Победы", 240, out var park_blue)
                .RelationTo(@"Славянский бульвар", 240)
                .RelationTo(@"Кунцевская", 120, out var kunz_blue)
                .RelationTo(@"Молодёжная", 170, out var molodezh)
                .RelationTo(@"Крылатское", 170)
                .RelationTo(@"Строгино", 480)
                .RelationTo(@"Мякинино", 240)
                .RelationTo(@"Волоколамская", 180)
                .RelationTo(@"Митино", 180)
                .RelationTo(@"Пятницкое шоссе", 150);

            // Бутовская
            butovo
                .AddStation(@"Битцевский парк", out var bitz_park)
                .RelationTo(@"Лесопарковая", 180)
                .RelationTo(@"Улица Старокачаловская", 180, out var starokach)
                .RelationTo(@"Улица Скобелевская", 300)
                .RelationTo(@"Бульвар адмирала Ушакова", 100)
                .RelationTo(@"Улица Горчакова", 120)
                .RelationTo(@"Бунинская аллея", 130);

            // Каховская
            kahovskaya
                .AddStation(@"Каширская", out var kashir_kah)
                .RelationTo(@"Варшавская", 120)
                .RelationTo(@"Каховская", 160, out var kah_kah);

            // Люблинско-Дмитровская
            lublino_dmitr
                .AddStation(@"Петровско-Разумовская", out var petr_razum_lubl)
                .RelationTo(@"Фонвизинская", 180, out var fonviz)
                .RelationTo(@"Бутырская", 180)
                .RelationTo(@"Марьина роща", 180)
                .RelationTo(@"Достоевская", 180)
                .RelationTo(@"Трубная", 180, out var trub)
                .RelationTo(@"Сретенский бульвар", 120, out var sret_bul)
                .RelationTo(@"Чкаловская", 180, out var chkal)
                .RelationTo(@"Римская", 150, out var rim)
                .RelationTo(@"Крестьянская застава", 140, out var krest_zast)
                .RelationTo(@"Дубровка", 120, out var dubrov)
                .RelationTo(@"Кожуховская", 120)
                .RelationTo(@"Печатники", 220)
                .RelationTo(@"Волжская", 140)
                .RelationTo(@"Люблино", 150)
                .RelationTo(@"Братиславская", 200)
                .RelationTo(@"Марьино", 120)
                .RelationTo(@"Борисово", 180)
                .RelationTo(@"Шипиловская", 120)
                .RelationTo(@"Зябликово", 120, out var zyablik);

            Scheme.AddRelation(borov, biblioteka, 240);             // Библиотека им. Ленина -> Боровицкая
            Scheme.AddRelation(serpuh, dobrin, 360);                // Серпуховская -> Добрынинская
            Scheme.AddRelation(park_kult_red, park_kul_ring, 300);  // Парк культуры -> Парк культуры
            Scheme.AddRelation(novoslob, mendeleev, 270);           // Менделеевская -> Новослободская
            Scheme.AddRelation(komsomol_red, komsomol_ring, 300);   // Комсомольская -> Комсомольская
            Scheme.AddRelation(aleks_sad, biblioteka, 300);         // Библиотека им. Ленина -> Александровский сад
            Scheme.AddRelation(kiev_fili, kiev_ring, 300);          // Киевская -> Киевская

            Scheme.AddRelation(belorus_green, belorus_ring, 300);   // Белорусская -> Белорусская
            Scheme.AddRelation(tver, chehov, 360);                  // Тверская -> Чеховская
            Scheme.AddRelation(teatral, ohotniy, 240);              // Театральная -> Охотный ряд
            Scheme.AddRelation(pavel_ring, pavel_green, 360);       // Павелецкая -> Павелецкая

            Scheme.AddRelation(kursk_blue,kursk_ring, 360);         // Курская -> Курская
            Scheme.AddRelation(revol, teatral, 240);                // Театральная -> Площадь Революции
            Scheme.AddRelation(arbat_blue, aleks_sad, 300);         // Арбатская -> Александровский сад
            Scheme.AddRelation(arbat_blue, borov, 240);             // Арбатская -> Боровицкая
            Scheme.AddRelation(kiev_blue, kiev_fili, 360);          // Киевская -> Киевская (фили)
            Scheme.AddRelation(kiev_blue, kiev_ring, 300);          // Киевская -> Киевская (кольцо)
            Scheme.AddRelation(kunz_blue, kunzevo_fili, 240);       // Кунцевская -> Кунцевская

            Scheme.AddRelation(starokach, don, 240);                // Бульвар Дмитрия Донского -> Улица Старокачаловская

            Scheme.AddRelation(kah_kah, sevastopol, 240);           // Севастопольская -> Каховская
            Scheme.AddRelation(kashir, kashir_kah, 240);            // Каширская -> Каширская

            Scheme.AddRelation(krasnogvard, zyablik, 360);          // Красногвардейская -> Зябликово
            Scheme.AddRelation(chkal, kursk_blue, 300);             // Чкаловская -> Курская (синяя)
            Scheme.AddRelation(chkal, kursk_ring, 360);             // Чкаловская -> Курская (кольцо)
            Scheme.AddRelation(sret_bul, prudi, 240);               // Чистые пруды -> Сретенский бульвар
            Scheme.AddRelation(trub, tzhvetnoy, 300);               // Цветной бульвар -> Трубная
            Scheme.AddRelation(petr_razum_lubl, petrovsko, 180);    // Петровско-Разумовская -> Петровско-Разумовская
        }
        /*
        private void WriteRoute(IRoute route)
        {
            System.Diagnostics.Debug.WriteLine($"---route from {route.From.Name} to {route.To.Name} timespan {route.Timespan}");
            //System.Diagnostics.Debug.WriteLine(string.Join(" ... ", route.GetLinesRelations().Select(rr => rr.From.Name + " > " + rr.To.Name)));
        }
        */
        private void WriteRoute(CompositeRoute route)
        {
            System.Diagnostics.Debug.WriteLine($"Маршрут: {route.From.Name} -> {route.To.Name}");
            Station prior = null;
            int counter = 0;

            foreach (var station in route)
            {
                if (station.Line != prior?.Line)
                {
                    if (counter > 0)
                        Debug.WriteLine($"({counter} станции)");
                    Debug.WriteLine($"{prior?.Name} ({prior?.Line.Name}) -> {station.Name} ({station.Line.Name})");
                    counter = 0;
                }
                else
                    ++counter;

                prior = station;
            }

            Debug.WriteLine($"Конец маршрута. Время в пути: {route.Timespan} секунд");
        }

        [TestMethod]
        public void TestFindRoute()
        {
            var from = Scheme.GetStations()
                .Where(s => s.Name == @"Бунинская аллея").First();
            var to = Scheme.GetStations()
                .Where(s => s.Name == @"Алтуфьево").First();
            var route = new SchemeVisitor().FindRoute(from, to);
        }

        [TestMethod]
        public void TestVisitAllLines()
        {
            var routes = new SchemeVisitor()
                .VisitAllLines(Scheme)
                .OrderBy(r => r.Timespan)
                .ToArray();
            var minRoutes = routes.Where(r => r.Timespan == routes.Min(rr => rr.Timespan)).ToArray();
            foreach (var r in minRoutes)
            {
                WriteRoute(r);
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Ln>>(System.IO.File.ReadAllText(@"D:\stations.json"));
            var rels = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Rel>>(System.IO.File.ReadAllText(@"D:\stations_rel.json"));

            var stations = obj.SelectMany(o => o.Stations).ToArray();
            var rrr = rels
                .Select(r => new
                {
                    Station1 = stations.FirstOrDefault(s => s.Id == r.Stations.First()).Name,
                    Station1Id = r.Stations.First(),
                    Station2 = stations.FirstOrDefault(s => s.Id == r.Stations.Last()).Name,
                    Station2Id = r.Stations.Last(),
                    Time = r.Time
                }).ToArray();
            var resultStr = Newtonsoft.Json.JsonConvert.SerializeObject(rrr);
            using (var writer = System.IO.File.CreateText(@"D:\stations_gen.txt"))
            {
                foreach (var o in obj)
                {
                    writer.WriteLine(@"----------------------НОВАЯ ВЕТКА--------------------------------");
                    writer.WriteLine(o.Name);
                    writer.WriteLine("-------станции");
                    foreach (var s in o.Stations)
                    {
                        writer.WriteLine(s.Name);

                        var s_rels = rrr
                            .Where(r => r.Station1Id == s.Id || r.Station2Id == s.Id)
                            .Select(r => new
                            {
                                Station1 = r.Station1,
                                Station2 = r.Station2,
                                Time = r.Time
                            }).ToArray();
                        foreach (var s_rel in s_rels)
                        {
                            writer.WriteLine($"    {s_rel.Station1} -> {s_rel.Station2}: {s_rel.Time}");
                        }
                    }
                }
            }

            using (var writer = System.IO.File.CreateText(@"D:\stations_rel_gen.txt"))
            {
                foreach (var r in rrr)
                {
                    writer.WriteLine("-----------");
                    writer.WriteLine(r.Station1);
                    writer.WriteLine(r.Station2);
                    writer.WriteLine(r.Time);
                    writer.WriteLine("-----------");
                }
            }
        }
    }
}
