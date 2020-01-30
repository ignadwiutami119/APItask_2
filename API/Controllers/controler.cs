using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Task.Models;

namespace Task.Controllers {
    [Route ("/task")]
    [ApiController]
    public class controler {
        MyDbContext _appDbContext;
        public controler (MyDbContext appDbContext) {
            this._appDbContext = appDbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Objek>> Get () {
            return _appDbContext.obj;
        }

        [HttpPost]
        public ActionResult<Objek> Post ([FromBody] Objek dt) {
            _appDbContext.Add (dt);
            _appDbContext.SaveChanges ();
            return dt;
        }

        [HttpPatch ("{id}")]
        public ActionResult<Objek> Update (int id, [FromBody] Objek request) {
            var get = _appDbContext.obj.Find (id);
            get.act = request.act;
            _appDbContext.SaveChanges ();
            return get;
        }

        [HttpPatch]
        [Route ("/task/update/{id}")]
        public ActionResult<Objek> UpdateStatus (int id, [FromBody] Objek request) {
            var get = _appDbContext.obj.Find (id);
            get.status = request.status;
            _appDbContext.SaveChanges ();
            return get;
        }

        [HttpDelete ("{id}")]
        public ActionResult<string> Delete (int id) {
            var dt = _appDbContext.obj.Find (id);
            _appDbContext.Attach (dt);
            _appDbContext.Remove (dt);
            _appDbContext.SaveChanges ();
            return $"Data that has id {id} was deleted";
        }
    }
}