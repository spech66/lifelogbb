using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LifelogBb.Models;
using LifelogBb.Models.Entities;
using AutoMapper;
using LifelogBb.Models.BucketLists;
using System.Net;
using LifelogBb.Utilities;

namespace LifelogBb.Controllers
{
    public class BucketListsController : Controller
    {
        private readonly LifelogBbContext _context;

        private readonly IConfiguration _configuration;

        protected readonly IMapper _mapper;

        public BucketListsController(LifelogBbContext context, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
        }

        // GET: BucketLists
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            var items = from s in _context.BucketLists select s;

            int pageSize = 20;
            return View(await PaginatedList<BucketList>.CreateAsync(items.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: /BucketLists/GetImage/2
        public async Task<IActionResult> GetImage(long? id)
        {
            if (id == null || _context.BucketLists == null)
            {
                return NotFound();
            }

            var bucketList = await _context.BucketLists.FirstOrDefaultAsync(m => m.Id == id);
            if (bucketList == null || bucketList.ImageName == null || bucketList.ImageFileName == null)
            {
                return NotFound();
            }

            // https://github.com/cymen/ApacheMimeTypesToDotNet/blob/master/ApacheMimeTypes.cs
            var type = "";
            switch (Path.GetExtension(bucketList.ImageName).ToLower())
            {
                case ".gif": type = "image/gif"; break;
                case ".jpeg": type = "image/jpeg"; break;
                case ".jpg": type = "image/jpeg"; break;
                case ".png": type = "image/png"; break;
                default: type = "application/octet-stream"; break;
            }

            var file = Path.Join(GetAndCreateBaseDir(), bucketList.ImageFileName);
            if (!System.IO.File.Exists(file))
            {
                return NotFound();
            }

            return PhysicalFile(file, type, bucketList.ImageName);
        }

        // GET: BucketLists/VisionBoard
        public async Task<IActionResult> VisionBoard()
        {
            var items = await _context.BucketLists.ToListAsync();
            return View(_mapper.Map<List<BucketList>>(items));
        }

        // GET: BucketLists/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.BucketLists == null)
            {
                return NotFound();
            }

            var bucketList = await _context.BucketLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bucketList == null)
            {
                return NotFound();
            }

            return View(bucketList);
        }

        // GET: BucketLists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BucketLists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("...")]*/CreateBucketListViewModel bucketListViewModel)
        {
            if (ModelState.IsValid)
            {
                var bucketListDb = new BucketList();
                bucketListDb = _mapper.Map(bucketListViewModel, bucketListDb);
                bucketListDb.SetCreateFields();

                if (bucketListViewModel.ImageData != null)
                {
                    if (!ValidateFileExtions(bucketListViewModel.ImageData.FileName))
                    {
                        ModelState.AddModelError("ImageData", "Invalid file extension.");
                        return View(bucketListViewModel);
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await bucketListViewModel.ImageData.CopyToAsync(memoryStream);

                        // Check size for 10 MB
                        if (memoryStream.Length < 10 * 1024 * 1024)
                        {
                            await SaveImageFileAsync(bucketListDb, bucketListViewModel.ImageData, memoryStream);
                        }
                        else
                        {
                            ModelState.AddModelError("ImageData", "The file is too large.");
                            return View(bucketListViewModel);
                        }
                    }
                }

                _context.Add(bucketListDb);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bucketListViewModel);
        }

        // GET: BucketLists/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.BucketLists == null)
            {
                return NotFound();
            }

            var bucketListDb = await _context.BucketLists.FindAsync(id);
            if (bucketListDb == null)
            {
                return NotFound();
            }
            var bucketList = _mapper.Map<EditBucketListViewModel>(bucketListDb);
            return View(bucketList);
        }

        // POST: BucketLists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, /*[Bind("...")]*/ EditBucketListViewModel bucketListViewModel)
        {
            if (id != bucketListViewModel.Id)
            {
                return NotFound();
            }

            var bucketListDb = await _context.BucketLists.FindAsync(id);
            if (ModelState.IsValid && bucketListDb != null)
            {
                try
                {
                    bucketListDb = _mapper.Map(bucketListViewModel, bucketListDb);
                    bucketListDb.SetUpdateFields();

                    if (bucketListViewModel.ImageData != null)
                    {
                        if (!ValidateFileExtions(bucketListViewModel.ImageData.FileName))
                        {
                            ModelState.AddModelError("ImageData", "Invalid file extension.");
                            return View(bucketListViewModel);
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            await bucketListViewModel.ImageData.CopyToAsync(memoryStream);

                            // Check size for 10 MB
                            if (memoryStream.Length < 10 * 1024 * 1024)
                            {
                                await SaveImageFileAsync(bucketListDb, bucketListViewModel.ImageData, memoryStream);
                            }
                            else
                            {
                                ModelState.AddModelError("ImageData", "The file is too large.");
                                return View(bucketListViewModel);
                            }
                        }
                    } else
                    {
                        // Delete existing file
                        if (bucketListDb.ImageFileName != null)
                        {
                            var file = Path.Join(GetAndCreateBaseDir(), bucketListDb.ImageFileName);
                            if(System.IO.File.Exists(file))
                            {
                                System.IO.File.Delete(file);
                            }

                            bucketListDb.ImageName = null;
                            bucketListDb.ImageFileName = null;
                        }
                    }

                    _context.Update(bucketListDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BucketListExists(bucketListViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bucketListViewModel);
        }

        // GET: BucketLists/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.BucketLists == null)
            {
                return NotFound();
            }

            var bucketList = await _context.BucketLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bucketList == null)
            {
                return NotFound();
            }

            return View(bucketList);
        }

        // POST: BucketLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.BucketLists == null)
            {
                return Problem("Entity set 'LifelogBbContext.BucketList'  is null.");
            }
            var bucketList = await _context.BucketLists.FindAsync(id);
            if (bucketList != null)
            {
                _context.BucketLists.Remove(bucketList);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BucketListExists(long id)
        {
          return _context.BucketLists.Any(e => e.Id == id);
        }

        private bool ValidateFileExtions(string fileName)
        {
            string[] permittedExtensions = { ".png", ".jpg", ".jpeg", ".gif", ".webp" };
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                ModelState.AddModelError("ImageData", "Invalid image type.");
                return false;
            }

            return true;
        }

        private async Task SaveImageFileAsync(BucketList bucketListDb, IFormFile imageData, MemoryStream memoryStream)
        {
            string untrustedFileName = Path.GetFileName(imageData.FileName);
            bucketListDb.ImageName = WebUtility.HtmlEncode(untrustedFileName);
            // Reuse existing filename
            if (bucketListDb.ImageFileName != null)
            {
                bucketListDb.ImageFileName = Guid.NewGuid().ToString();
            }

            var file = Path.Join(GetAndCreateBaseDir(), bucketListDb.ImageFileName);

            using Stream fileStream = System.IO.File.Open(file, FileMode.Create);
            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(fileStream);
        }

        private string GetAndCreateBaseDir()
        {
            var path = _configuration["Uploads:Path"];
            if (string.IsNullOrEmpty(path))
            {
                var folder = Environment.SpecialFolder.LocalApplicationData;
                path = Environment.GetFolderPath(folder);
                path = Path.Join(path, "lifelogbb", "uploads", "bucketlist");
            }
            if (!Path.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}
