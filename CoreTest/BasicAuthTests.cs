﻿using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.TeamFoundation.Git.Helpers.Authentication
{
    [TestClass]
    public class BasicAuthTests
    {
        public BasicAuthTests()
        {
            Trace.Listeners.AddRange(Debug.Listeners);
        }

        [TestMethod]
        public void BasicAuthDeleteCredentialsTest()
        {
            Uri targetUri = new Uri("http://localhost");
            BasicAuthentication basicAuth = GetBasicAuthentication("basic-delete");

            basicAuth.CredentialCache.WriteCredentials(targetUri, new Credential("username", "password"));
            basicAuth.CredentialStore.WriteCredentials(targetUri, new Credential("username", "password"));

            Credential credentials;

            basicAuth.DeleteCredentials(targetUri);

            Assert.IsFalse(basicAuth.CredentialCache.ReadCredentials(targetUri, out credentials), "User credentials were not deleted as expected");
            Assert.IsFalse(basicAuth.CredentialStore.ReadCredentials(targetUri, out credentials), "User credentials were not deleted as expected");
        }

        [TestMethod]
        public void BasicAuthGetCredentialsTest()
        {
            Uri targetUri = new Uri("http://localhost");
            BasicAuthentication basicAuth = GetBasicAuthentication("basic-get");

            Credential credentials = null;

            Assert.IsFalse(basicAuth.GetCredentials(targetUri, out credentials), "User credentials were unexpectedly retrieved.");

            credentials = new Credential("username", "password");

            basicAuth.CredentialStore.WriteCredentials(targetUri, credentials);

            Assert.IsTrue(basicAuth.GetCredentials(targetUri, out credentials), "User credentials were unexpectedly not retrieved.");
            Assert.IsTrue(basicAuth.CredentialCache.ReadCredentials(targetUri, out credentials), "User credentials were unexpectedly not found in the cache.");
        }

        [TestMethod]
        public void BasicAuthSetCredentialsTest()
        {
            Uri targetUri = new Uri("http://localhost");
            BasicAuthentication basicAuth = GetBasicAuthentication("basic-set");

            Credential credentials = null;

            Assert.IsFalse(basicAuth.GetCredentials(targetUri, out credentials), "User credentials were unexpectedly retrieved.");
            try
            {
                basicAuth.SetCredentials(targetUri, credentials);
                Assert.Fail("User credentials were unexpectedly set.");
            }
            catch { }

            credentials = new Credential("username", "password");

            Assert.IsTrue(basicAuth.SetCredentials(targetUri, credentials), "User credentials were unexpectedly not set.");
            Assert.IsTrue(basicAuth.GetCredentials(targetUri, out credentials), "User credentials were unexpectedly not retrieved.");
        }

        private BasicAuthentication GetBasicAuthentication(string prefix)
        {
            ICredentialStore credentialStore = new CredentialCache(prefix);
            ICredentialStore credentialCache = new CredentialCache(prefix);

            return new BasicAuthentication(credentialStore, credentialCache);
        }
    }
}