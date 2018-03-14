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
            #region Схема московского метро

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
            var kaluzhsko_rizh = Scheme.AddLine(@"Калужско-Рижская");
            var tagansko_krasnopr = Scheme.AddLine(@"Таганско-Краснопресненская");
            var kalin = Scheme.AddLine(@"Калининская");

            #region Серпуховско-Тимирязевская
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
            #endregion

            #region Замоскворецкая
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
                .RelationTo(@"Новокузнецкая", 120, out var novokuz)
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
            #endregion

            #region Сокольническая
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
            #endregion
            
            #region Кольцевая
            ring
                .AddStation(@"Парк культуры", out var park_kul_ring)
                .RelationTo(@"Октябрьская", 100, out var oktyabr)
                .RelationTo(@"Добрынинская", 90, out var dobrin)
                .RelationTo(@"Павелецкая", 90, out var pavel_ring)
                .RelationTo(@"Таганская", 110, out var tagan_ring)
                .RelationTo(@"Курская", 140, out var kursk_ring)
                .RelationTo(@"Комсомольская", 160, out var komsomol_ring)
                .RelationTo(@"Проспект Мира", 140, out var prosp_mira_ring)
                .RelationTo(@"Новослободская", 130, out var novoslob)
                .RelationTo(@"Белорусская", 110, out var belorus_ring)
                .RelationTo(@"Краснопресненская", 140, out var krasnopres)
                .RelationTo(@"Киевская", 150, out var kiev_ring);
            #endregion
            
            #region Филёвская
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
            #endregion
            
            #region Арбатско-Покровская
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
                .RelationTo(@"Парк Победы", 240, out var park_pobedi_blue)
                .RelationTo(@"Славянский бульвар", 240)
                .RelationTo(@"Кунцевская", 120, out var kunz_blue)
                .RelationTo(@"Молодёжная", 170, out var molodezh)
                .RelationTo(@"Крылатское", 170)
                .RelationTo(@"Строгино", 480)
                .RelationTo(@"Мякинино", 240)
                .RelationTo(@"Волоколамская", 180)
                .RelationTo(@"Митино", 180)
                .RelationTo(@"Пятницкое шоссе", 150);
            #endregion
            
            #region Бутовская
            butovo
                .AddStation(@"Битцевский парк", out var bitz_park)
                .RelationTo(@"Лесопарковая", 180)
                .RelationTo(@"Улица Старокачаловская", 180, out var starokach)
                .RelationTo(@"Улица Скобелевская", 300)
                .RelationTo(@"Бульвар адмирала Ушакова", 100)
                .RelationTo(@"Улица Горчакова", 120)
                .RelationTo(@"Бунинская аллея", 130);
            #endregion
            
            #region Каховская
            kahovskaya
                .AddStation(@"Каширская", out var kashir_kah)
                .RelationTo(@"Варшавская", 120)
                .RelationTo(@"Каховская", 160, out var kah_kah);
            #endregion

            #region Люблинско-Дмитровская
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
            #endregion

            #region Калужско-Рижская

            kaluzhsko_rizh
                .AddStation(@"Медведково")
                .RelationTo(@"Бабушкинская", 150)
                .RelationTo(@"Свиблово", 140)
                .RelationTo(@"Ботанический сад", 110, out var botan_sad)
                .RelationTo(@"ВДНХ", 190, out var vdnh)
                .RelationTo(@"Алексеевская", 110)
                .RelationTo(@"Рижская", 120)
                .RelationTo(@"Проспект Мира", 110, out var prosp_mira_kaluzh)
                .RelationTo(@"Сухаревская", 90)
                .RelationTo(@"Тургеневская", 80, out var turgen)
                .RelationTo(@"Китай-город", 100, out var kitay_kaluzh)
                .RelationTo(@"Третьяковская", 150, out var tretyak_kaluzh)
                .RelationTo(@"Октябрьская", 130, out var oktyabr_kaluzh)
                .RelationTo(@"Шаболовская", 100)
                .RelationTo(@"Ленинский проспект", 170, out var lenin_prosp)
                .RelationTo(@"Академическая", 170)
                .RelationTo(@"Профсоюзная", 110)
                .RelationTo(@"Новые Черёмушки", 90)
                .RelationTo(@"Калужская", 120)
                .RelationTo(@"Беляево", 150)
                .RelationTo(@"Коньково", 90)
                .RelationTo(@"Тёплый стан", 130)
                .RelationTo(@"Ясенево", 160)
                .RelationTo(@"Новоясеневская", 130, out var novoyas);

            #endregion

            #region Таганско-Краснопресненская

            tagansko_krasnopr
                .AddStation(@"Планерная")
                .RelationTo(@"Сходненская", 100)
                .RelationTo(@"Тушинская", 190)
                .RelationTo(@"Спартак", 120)
                .RelationTo(@"Щукинская", 120)
                .RelationTo(@"Октябрьское поле", 180, out var oktyabr_pole)
                .RelationTo(@"Полежаевская", 190, out var polezh)
                .RelationTo(@"Беговая", 130)
                .RelationTo(@"Улица 1905 года", 110)
                .RelationTo(@"Баррикадная", 110, out var barrikad)
                .RelationTo(@"Пушкинская", 140, out var pushkin)
                .RelationTo(@"Кузнецкий мост", 100, out var kuznez)
                .RelationTo(@"Китай-город", 70, out var kitay_t)
                .RelationTo(@"Таганская", 150, out var tagan_t)
                .RelationTo(@"Пролетарская", 120, out var prolet)
                .RelationTo(@"Волгоградский проспект", 130)
                .RelationTo(@"Текстильщики", 230)
                .RelationTo(@"Кузьминки", 160)
                .RelationTo(@"Рязанский проспект", 180)
                .RelationTo(@"Выхино", 150)
                .RelationTo(@"Лермонтовский проспект", 240)
                .RelationTo(@"Жулебино", 180)
                .RelationTo(@"Котельники", 120);

            #endregion

            #region Калининская

            kalin
                .AddStation(@"Новокосино")
                .RelationTo(@"Новогиреево", 180)
                .RelationTo(@"Перово", 140)
                .RelationTo(@"Шоссе Энтузиастов", 220)
                .RelationTo(@"Авиамоторная", 130, out var aviamotor)
                .RelationTo(@"Площадь Ильича", 160, out var ploshad_ilicha)
                .RelationTo(@"Марксистская", 150, out var marks)
                .RelationTo(@"Третьяковская", 140, out var tretyak_kalin);
            kalin
                .AddStation(@"Раменки")
                .RelationTo(@"Ломоносовский проспект", 120)
                .RelationTo(@"Минская", 240)
                .RelationTo(@"Парк Победы", 240, out var park_pobedi_kalin)
                .RelationTo(@"Шелепиха", 270, out var shelep)
                .RelationTo(@"Хорошевская", 250, out var horoshevskaya)
                .RelationTo(@"ЦСКА", 140, out var cska_kalin)
                .RelationTo(@"Петровский парк", 160, out var petrov_park);

            #endregion

            #region Переходы между ветками

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

            Scheme.AddRelation(prosp_mira_kaluzh, prosp_mira_ring, 300);    // Проспект Мира -> Проспект Мира
            Scheme.AddRelation(turgen, prudi, 300);                         // Тургеневская -> Чистые пруды
            Scheme.AddRelation(turgen, sret_bul, 300);                      // Тургеневская -> Сретенский бульвар
            Scheme.AddRelation(oktyabr_kaluzh, oktyabr, 300);               // Октябрьская -> Октябрьская
            Scheme.AddRelation(novoyas, bitz_park, 120);                    // Новоясеневская -> Битцевский парк

            Scheme.AddRelation(barrikad,krasnopres, 330);           // Баррикадная -> Краснопресненская
            Scheme.AddRelation(pushkin,tver, 360);                  // Тверская -> Пушкинская
            Scheme.AddRelation(pushkin,chehov, 360);                // Пушкинская -> Чеховская
            Scheme.AddRelation(kuznez, lubyanka, 240);              // Кузнецкий мост -> Лубянка
            Scheme.AddRelation(kitay_t, kitay_kaluzh, 120);         // Китай-город -> Китай-город
            Scheme.AddRelation(tagan_t, tagan_ring, 360);           // Таганская -> Таганская
            Scheme.AddRelation(prolet,krest_zast, 330);             // Пролетарская -> Крестьянская застава

            Scheme.AddRelation(ploshad_ilicha, rim, 360);           // Площадь Ильича -> Римская
            Scheme.AddRelation(marks, tagan_ring, 240);             // Марксистская -> Таганская (кольцо)
            Scheme.AddRelation(marks, tagan_t, 390);                // Таганская -> Марксистская

            Scheme.AddRelation(tretyak_kalin, tretyak_kaluzh, 180); // Третьяковская -> Третьяковская
            Scheme.AddRelation(tretyak_kalin, novokuz, 240);        // Третьяковская -> Новокузнецкая
            Scheme.AddRelation(park_pobedi_kalin, park_pobedi_blue, 60);    // Парк Победы -> Парк Победы
            Scheme.AddRelation(horoshevskaya, polezh, 180);         // Хорошевская -> Полежаевская

            #endregion

            #endregion
        }

        private void WriteRoute(IRoute route)
        {
            Debug.WriteLine($"Маршрут: {route.From.Name} -> {route.To.Name}");
            int counter = 0;
            int timespan = 0;
            foreach (var r in route.GetRoutes(false))
            {
                if (r.From.Line == r.To.Line)
                {
                    timespan += r.Timespan;
                    counter++;
                }
                else
                {
                    if (counter > 0)
                    {
                        Debug.WriteLine($"  {counter} станции(й) ({timespan} секунд)");
                        counter = 0;
                        timespan = 0;
                    }

                    Debug.WriteLine($"{r.From.Name} ({r.From.Line.Name}) -> {r.To.Name} ({r.To.Name}) ({r.Timespan} секунд)");
                }
            }
            if (counter > 0)
            {
                    Debug.WriteLine($"  {counter} станции(й) ({timespan} секунд)");
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
            WriteRoute(route);
        }

        [TestMethod]
        public void TestVisitAllLines()
        {
            var routes = new SchemeVisitor().VisitAllLines(Scheme);            
            foreach (var r in routes)
            {
                WriteRoute(r);
            }
        }

        [TestMethod]
        public void TestVisitAllLinesByBranchAndBound()
        {
            var routes = new SchemeVisitor().VisitAllLinesByBranchAndBound(Scheme);
            foreach (var r in routes)
            {
                WriteRoute(r);
            }
        }

        [TestMethod]
        public void TestRoute()
        {
            var scheme = new Scheme();
            var line1 = scheme.AddLine(@"line1");
            var line2 = scheme.AddLine(@"line2");
            var line3 = scheme.AddLine(@"line3");
            var line4 = scheme.AddLine(@"line4");
            var line5 = scheme.AddLine(@"line5");
            var s1 = line1.AddStation(@"s1");
            var s2 = line2.AddStation(@"s2");
            var s3 = line3.AddStation(@"s3");
            var s4 = line4.AddStation(@"s4");
            var s5 = line5.AddStation(@"s5");
            var s1s2 = scheme.AddRelation(s1, s2, 20);
            var s1s3 = scheme.AddRelation(s1, s3, 18);
            var s1s4 = scheme.AddRelation(s1, s4, 12);
            var s1s5 = scheme.AddRelation(s1, s4, 8);
            var s2s3 = scheme.AddRelation(s2, s3, 14);
            var s2s4 = scheme.AddRelation(s2, s4, 7);
            var s2s5 = scheme.AddRelation(s2, s5, 11);
            var s3s4 = scheme.AddRelation(s3, s4, 6);
            var s3s5 = scheme.AddRelation(s3, s5, 11);
            var s4s5 = scheme.AddRelation(s4, s5, 12);

            var routes = new List<IRoute>();
            routes.Add(s1s2);
            routes.Add(s1s3);
            routes.Add(s1s4);
            routes.Add(s1s5);
            routes.Add(s2s3);
            routes.Add(s2s4);
            routes.Add(s2s5);
            routes.Add(s3s4);
            routes.Add(s3s5);
            routes.Add(s4s5);

            var r = new SchemeVisitor().VisitAllLinesByBranchAndBound(scheme);
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
