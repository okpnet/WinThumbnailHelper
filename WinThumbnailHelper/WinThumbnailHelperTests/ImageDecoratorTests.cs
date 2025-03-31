using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinThumbnailHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace WinThumbnailHelper.Tests
{
    [TestClass()]
    public class ImageDecoratorTests
    {
        [TestMethod()]
        public async void SetTumbnailTest()
        {
            var imagePath = "";
            try
            {
                var imageDeco = await ImageDecorator.CreateAsync(imagePath, 100);
                if (imageDeco.Exceptions is not null)
                {
                    var dirPath = Assembly.GetExecutingAssembly().Location;
                    var path = System.IO.Path.Combine(dirPath, $"{nameof(SetTumbnailTest)}.bmp");
                    imageDeco.Thumbnail.Save(path);
                }
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void SetTumbnailTest1()
        {
            var failImagePath = "C:\\Users\\htakahashi\\Downloads\\abc.bmp";//存在しない
            var imageSuccessPath = "C:\\Users\\htakahashi\\Downloads\\abcd.bmp";//存在する
            try
            {
                var failImageDeco = ImageDecorator.CreateAsync(failImagePath, 100).Result;
                if(failImageDeco.Exceptions is null)
                {
                    throw new ArgumentNullException("failImageDeco has not exception");
                }
                var dir = System.IO.Path.GetDirectoryName(failImagePath);
                var filePath = System.IO.Path.Combine(dir!, $"fail_{nameof(SetTumbnailTest1)}.bmp");
                failImageDeco.Thumbnail.Save(filePath);

                var successImageDeco= ImageDecorator.CreateAsync(imageSuccessPath, 50).Result;
                if (successImageDeco.Exceptions is not null)
                {
                    throw successImageDeco.Exceptions;
                }
                var successFilePath = System.IO.Path.Combine(dir!, $"{nameof(SetTumbnailTest1)}.bmp");
                successImageDeco.Thumbnail.Save(successFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void CreateAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateAsyncTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DisposeTest()
        {
            Assert.Fail();
        }
    }
}