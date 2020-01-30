using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace Task {

    class Program {
        public static int Main (string[] args) {
            var Modify_json = new CommandLineApplication () {
                Name = "modify_json",
                Description = "it should modify json data",
                ShortVersionGetter = () => "1.0.0"
            };

            Modify_json.Command ("modify", app => {
                app.Description = "change json data";
                var show = app.Option ("--list", "show data", CommandOptionType.SingleOrNoValue);
                var clear = app.Option ("--clear", "clear all data", CommandOptionType.SingleOrNoValue);
                var add = app.Option ("--add", "add data", CommandOptionType.MultipleValue);
                var update = app.Option ("--update", "upate spesific data", CommandOptionType.SingleOrNoValue);
                var delete = app.Option ("--delete", "delete spesific data", CommandOptionType.SingleOrNoValue);
                var done = app.Option ("--done", "change activity status", CommandOptionType.SingleOrNoValue);
                app.OnExecuteAsync (async cancellationToken => {
                    HttpClientHandler clHandler = new HttpClientHandler();
                    clHandler.ServerCertificateCustomValidationCallback = (sender, sert, chain, sslPolicyErrors) => { return true;};
                    HttpClient client = new HttpClient (clHandler);
                    HttpRequestMessage req = new HttpRequestMessage (HttpMethod.Get, "https://localhost:5001/task");
                    HttpResponseMessage resp = await client.SendAsync (req);
                    var jsondata = await resp.Content.ReadAsStringAsync ();
                    var data = JsonConvert.DeserializeObject<List<objek>> (jsondata);
                    if (show.HasValue ()) {
                        foreach (var item in data) {
                            Console.WriteLine (item.id + ". " + item.act);
                        }
                    }

                    if (clear.HasValue ()) {
                        var sure = Prompt.GetYesNo ("delete all data?", false);
                        foreach (var item in data) {
                            await client.DeleteAsync ("https://localhost:5001/task/" + item.id);
                        }
                        Console.WriteLine ("Data empty");
                    }

                    if (delete.HasValue ()) {
                        var del = Convert.ToInt32 (delete.Value ());
                        await client.DeleteAsync ("https://localhost:5001/task/" + del);
                        Console.WriteLine ($"Data that has id : {del} was deleted");
                    }

                    if (update.HasValue ()) {
                        Console.WriteLine ("Enter your update : ");
                        var input = Console.ReadLine ();
                        objek obj = new objek () {
                            act = input
                        };
                        var toJson = JsonConvert.SerializeObject (obj);
                        var cnt = new StringContent (toJson, Encoding.UTF8, "application/json");
                        var id = Convert.ToInt32 (update.Value ());
                        foreach (var item in data) {
                            if (item.id == id) {
                                await client.PatchAsync ("https://localhost:5001/task/"+ item.id, cnt);
                            }
                        }
                    }

                    if (done.HasValue ()) {
                        var id = Convert.ToInt32 (done.Value ());
                        foreach (var item in data) {
                            if (item.id == id) {
                                objek obj = new objek () {
                                id = item.id,
                                act = item.act,
                                status = "Done"
                                };
                                var toJson = JsonConvert.SerializeObject (obj);
                                var cnt = new StringContent (toJson, Encoding.UTF8, "application/json");
                                await client.PatchAsync ("https://localhost:5001/task/update/" + item.id, cnt);
                                Console.WriteLine ("status number " + item.id + " done");
                            }
                        }
                    }

                    if (add.HasValue ()) {
                        var objek = new objek () {
                            id = Convert.ToInt32 (add.Values[0]),
                            act = add.Values[1]
                        };
                        var toJson = JsonConvert.SerializeObject (objek);
                        var cnt = new StringContent (toJson, Encoding.UTF8, "application/json");
                        var posts = await client.PostAsync ("https://localhost:5001/task", cnt);
                    }
                });
            });

            Modify_json.OnExecute (() => {
                Modify_json.ShowHelp ();
            });
            return Modify_json.Execute (args);
        }
    }

    class objek {
        public int id { get; set; }
        public string act { get; set; }
        public string status { get; set; } = "undone";
    }
}