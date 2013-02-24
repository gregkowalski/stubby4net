﻿using System.Net;
using NUnit.Framework;
using stubby;

namespace integration
{
   [TestFixture]
   public class ViaFileTest {
      private readonly Stubby _stubby = new Stubby(new Arguments {
         Admin = 9999,
         Stubs = 9992,
         Data = "../../YAML/e2e.yaml"
      });

      [TestFixtureSetUp]
      public void Before() {
         _stubby.Start();
      }

      [TestFixtureTearDown]
      public void After() {
         _stubby.Stop();
      }

      [Test]
      public void BasicGET_ShouldReturnOK() {
         var request = WebRequest.Create("http://localhost:9992/basic/get");
         var response = (HttpWebResponse) request.GetResponse();
         response.Close();
         Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
      }

      [Test]
      public void BasicPUT_ShouldReturnOK() {
         var request = WebRequest.Create("http://localhost:9992/basic/put");
         request.Method = "Put";
         request.ContentLength = 0;
         var response = (HttpWebResponse) request.GetResponse();
         response.Close();
         Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
      }

      [Test]
      public void BasicPOST_ShouldReturnOK() {
         var request = WebRequest.Create("http://localhost:9992/basic/post");
         request.Method = "POST";
         request.ContentLength = 0;
         var response = (HttpWebResponse) request.GetResponse();
         response.Close();
         Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
      }

      [Test]
      public void BasicDELETE_ShouldReturnOK() {
         var request = WebRequest.Create("http://localhost:9992/basic/delete");
         request.Method = "DELETE";
         var response = (HttpWebResponse) request.GetResponse();
         response.Close();
         Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
      }

      [Test]
      public void BasicHEAD_ShouldReturnOK() {
         var request = WebRequest.Create("http://localhost:9992/basic/head");
         request.Method = "head";
         var response = (HttpWebResponse) request.GetResponse();
         response.Close();
         Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
      }

      [Test]
      public void EndpointRespondsTo_GET_FromAListOfManyMethods() {
         var request = WebRequest.Create("http://localhost:9992/basic/all");
         request.Method = "GEt";
         var response = (HttpWebResponse) request.GetResponse();
         response.Close();
         Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
      }

      [Test]
      public void EndpointRespondsTo_HEAD_FromAListOfManyMethods() {
         var request = WebRequest.Create("http://localhost:9992/basic/all");
         request.Method = "head";
         var response = (HttpWebResponse) request.GetResponse();
         response.Close();
         Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
      }

      [Test]
      public void EndpointRespondsTo_PUT_FromAListOfManyMethods() {
         var request = WebRequest.Create("http://localhost:9992/basic/all");
         request.Method = "put";
         request.ContentLength = 0;
         var response = (HttpWebResponse) request.GetResponse();
         response.Close();
         Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
      }

      [Test]
      public void EndpointRespondsTo_POST_FromAListOfManyMethods() {
         var request = WebRequest.Create("http://localhost:9992/basic/all");
         request.Method = "post";
         request.ContentLength = 0;
         var response = (HttpWebResponse) request.GetResponse();
         response.Close();
         Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
      }

      [Test]
      public void EndpointRespondsTo_DELETE_FromAListOfManyMethods() {
         var request = WebRequest.Create("http://localhost:9992/basic/all");
         request.Method = "DEleTE";
         var response = (HttpWebResponse) request.GetResponse();
         response.Close();
         Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
      }

      [Test]
      public void EndpointRespondsWith_body() {
         const string expectedBody = "plain text";
         var request = WebRequest.Create("http://localhost:9992/get/body");

         var response = (HttpWebResponse) request.GetResponse();
         var actualBody = TestUtils.ExtractBody(response);

         Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
         Assert.AreEqual(expectedBody, actualBody);
      }

      [Test]
      public void EndpointRespondsWith_Json() {
         const string expectedBody = "{\"property\":\"value\"}";
         var request = WebRequest.Create("http://localhost:9992/get/json");

         var response = (HttpWebResponse) request.GetResponse();
         var actualBody = TestUtils.ExtractBody(response);

         Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
         Assert.AreEqual("application/json", response.ContentType);
         Assert.AreEqual(expectedBody, actualBody);
      }

      [Test]
      public void EndpointRespondsWith_CustomClientErrorCode() {
         var request = WebRequest.Create("http://localhost:9992/get/420");

         try {
            request.GetResponse();
            Assert.Fail();
         } catch (WebException e) {
            var response = (HttpWebResponse) e.Response;
            Assert.AreEqual(420, (int) response.StatusCode);
         }
      }

      [Test]
      public void EndpointRespondsTo_QueryParameters() {
         const string expectedBody = "first query";
         var request = WebRequest.Create("http://localhost:9992/get/query?second=value2&first=value1");
         var response = (HttpWebResponse) request.GetResponse();
         var actualBody = TestUtils.ExtractBody(response);
         
         Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
         Assert.AreEqual(expectedBody, actualBody);
      }

      [Test]
      public void EndpointRespondsTo_DifferentQueryParameters_ForSameUrl() {
         const string expectedBody = "second query";
         var request = WebRequest.Create("http://localhost:9992/get/query?first=value1again&second=value2again");
         var response = (HttpWebResponse) request.GetResponse();
         var actualBody = TestUtils.ExtractBody(response);
         
         Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
         Assert.AreEqual(expectedBody, actualBody);
      }

      [Test]
      public void BasicAuthroization_IsConfigurableWith_Base64() {
         const string expectedBody = "resource has been created";
         var request = WebRequest.Create("http://localhost:9992/post/auth");
         request.Method = "post";
         request.Headers.Add(HttpRequestHeader.Authorization, TestUtils.EncodeBasicAuth("stubby", "password"));
         TestUtils.WritePost(request, "some=data");

         var response = (HttpWebResponse) request.GetResponse();
         var actualBody = TestUtils.ExtractBody(response);
         
         Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
         Assert.AreEqual(expectedBody, actualBody);
      }
   }
}