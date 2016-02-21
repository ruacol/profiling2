using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Infrastructure.Util;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;
using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    public class PersonPhotosController : BaseController
    {
        protected readonly IPersonTasks personTasks;
        protected readonly IPhotoTasks photoTasks;

        public PersonPhotosController(IPersonTasks personTasks, IPhotoTasks photoTasks)
        {
            this.personTasks = personTasks;
            this.photoTasks = photoTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public void Get(int personId, int photoId)
        {
            Person person = this.personTasks.GetPerson(personId);
            Photo photo = this.photoTasks.GetPhoto(photoId);
            if (person != null && photo != null && photo.FileData != null)
            {
                Response.ContentType = "image/jpeg";  // TODO file name/extension not always stored
                Response.OutputStream.Write(photo.FileData, 0, photo.FileData.Length);
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That person or photo doesn't exist.";
            }
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Edit(int personId, int photoId)
        {
            Photo photo = this.photoTasks.GetPhoto(photoId);
            Person person = this.personTasks.GetPerson(personId);
            if (photo != null && person != null)
            {
                PhotoViewModel vm = new PhotoViewModel(photo);
                vm.Id = photo.Id;
                vm.PhotoName = photo.PhotoName;
                vm.FileURL = photo.FileURL;
                vm.Notes = photo.Notes;
                vm.PersonId = personId;
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult Edit(PhotoViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Photo photo = this.photoTasks.GetPhoto(vm.Id);
                Person person = this.personTasks.GetPerson(vm.PersonId);
                if (photo != null)
                {
                    Photo existing = this.photoTasks.GetPhoto(vm.PhotoName);
                    if (existing != null && existing.Id != vm.Id)
                    {
                        ModelState.AddModelError("PhotoName", "Photo name already exists.");
                    }
                    else
                    {
                        photo.PhotoName = vm.PhotoName;
                        photo.FileURL = vm.FileURL;
                        photo.Notes = vm.Notes;
                        photo = this.photoTasks.SavePersonPhoto(person, photo);
                        return JsonNet(string.Empty);
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Person photo not found.");
                }
            }
            return JsonNet(this.GetErrorsForJson());
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult Delete(int personId, int photoId)
        {
            Photo photo = this.photoTasks.GetPhoto(photoId);
            Person person = this.personTasks.GetPerson(personId);
            if (photo != null && person != null)
            {
                this.photoTasks.DeletePersonPhoto(person, photo);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Photo successfully deleted.");
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Person or photo not found.");
            }
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Add(int personId)
        {
            Person p = this.personTasks.GetPerson(personId);
            if (p != null)
            {
                PhotoViewModel vm = new PhotoViewModel();
                vm.PersonId = p.Id;
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult Add(PhotoViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (this.photoTasks.GetPhoto(vm.PhotoName) == null)
                {
                    Person person = this.personTasks.GetPerson(vm.PersonId);
                    if (person != null)
                    {
                        Photo photo = vm.Id > 0 ? this.photoTasks.GetPhoto(vm.Id) : new Photo();
                        if (photo.FileData != null)
                        {
                            photo.PhotoName = vm.PhotoName;
                            photo.FileURL = vm.FileURL;
                            photo.Notes = vm.Notes;
                            photo = this.photoTasks.SavePersonPhoto(person, photo);
                            return JsonNet(string.Empty);
                        }
                        else
                            ModelState.AddModelError("FileData", "No photo was uploaded.");
                    }
                    else
                        ModelState.AddModelError("Person", "Person not found.");
                }
                else
                    ModelState.AddModelError("PhotoName", "Photo name already exists.");
            }
            return JsonNet(this.GetErrorsForJson());
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult Upload(HttpPostedFileBase FileData)
        {
            if (FileData != null && FileData.ContentLength > 0)
            {
                string contentType = MIMEAssistant.GetMIMEType(FileData.FileName);
                if (!string.IsNullOrEmpty(contentType) && !contentType.StartsWith("image"))
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return JsonNet("File received wasn't an image.");
                }

                if (this.photoTasks.GetPhoto(FileData.FileName) != null)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return JsonNet("File name already exists.");
                }

                Photo p = new Photo();
                p.PhotoName = FileData.FileName;
                p.FileName = FileData.FileName;
                p.FileTimeStamp = DateTime.Now;

                using (Stream inputStream = FileData.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    p.FileData = memoryStream.ToArray();
                }

                p = this.photoTasks.SavePhoto(p);
                return JsonNet(new
                {
                    Id = p.Id,
                    FileName = p.FileName
                });
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return JsonNet("Didn't receive any file.");
        }
    }
}