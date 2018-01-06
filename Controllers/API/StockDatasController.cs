using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using StockMarket.Models;

namespace StockMarket.Controllers.API {
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class StockDatasController : ODataController {
        private StockDataEntities db = new StockDataEntities();

        // GET odata/StockDatas
        [EnableQuery]
        public IQueryable<StockData> GetStockDatas() {
            return db.StockDatas;
        }

        // GET odata/StockDatas(5)
        [EnableQuery]
        public SingleResult<StockData> GetStockData([FromODataUri] int key) {
            return SingleResult.Create(db.StockDatas.Where(stockdata => stockdata.CompanyID == key));
        }

        // PUT odata/StockDatas(5)
        public IHttpActionResult Put([FromODataUri] int key, StockData stockdata) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (key != stockdata.CompanyID) {
                return BadRequest();
            }

            db.Entry(stockdata).State = EntityState.Modified;

            try {
                db.SaveChanges();
            } catch (DbUpdateConcurrencyException) {
                if (!StockDataExists(key)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return Updated(stockdata);
        }

        // POST odata/StockDatas
        public IHttpActionResult Post(StockData stockdata) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            db.StockDatas.Add(stockdata);

            try {
                db.SaveChanges();
            } catch (DbUpdateException) {
                if (StockDataExists(stockdata.CompanyID)) {
                    return Conflict();
                } else {
                    throw;
                }
            }

            return Created(stockdata);
        }

        // PATCH odata/StockDatas(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<StockData> patch) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            StockData stockdata = db.StockDatas.Find(key);
            if (stockdata == null) {
                return NotFound();
            }

            patch.Patch(stockdata);

            try {
                db.SaveChanges();
            } catch (DbUpdateConcurrencyException) {
                if (!StockDataExists(key)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return Updated(stockdata);
        }

        // DELETE odata/StockDatas(5)
        public IHttpActionResult Delete([FromODataUri] int key) {
            StockData stockdata = db.StockDatas.Find(key);
            if (stockdata == null) {
                return NotFound();
            }

            db.StockDatas.Remove(stockdata);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET odata/StockDatas(5)/Company
        [EnableQuery]
        public SingleResult<Company> GetCompany([FromODataUri] int key) {
            return SingleResult.Create(db.StockDatas.Where(m => m.CompanyID == key).Select(m => m.Company));
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StockDataExists(int key) {
            return db.StockDatas.Count(e => e.CompanyID == key) > 0;
        }
    }
}
