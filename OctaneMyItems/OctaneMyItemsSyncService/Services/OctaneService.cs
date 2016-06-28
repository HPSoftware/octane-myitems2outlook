﻿using OctaneMyItemsSyncService.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OctaneMyItemsSyncService.Services
{
    public class OctaneService
    {
        private readonly string _octaneServer;
        private HttpClient _httpClient;

        private string _currentUserName;
        private User _currentUser;
        private int _defaultSharespaceId;
        private Workspace _defaultWorkspace;

        public OctaneService(string octaneServer)
        {
            _octaneServer = octaneServer;
        }

        public async Task Login(string user, string password)
        {
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri(_octaneServer);
            var content = new StringContent(
                string.Format("{{\"user\":\"{0}\", \"password\":\"{1}\"}}", user, password),
                Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync("/authentication/sign_in", content);
            result.EnsureSuccessStatusCode();

            foreach (Cookie cookie in cookieContainer.GetCookies(result.RequestMessage.RequestUri))
            {
                if (cookie.Name == "HPSSO_COOKIE_CSRF")
                {
                    _httpClient.DefaultRequestHeaders.Add("HPSSO_HEADER_CSRF", cookie.Value);
                    _httpClient.DefaultRequestHeaders.Add("HPSSO-HEADER-CSRF", cookie.Value);
                }
                else if (cookie.Name == "LWSSO_COOKIE_KEY")
                {
                    _httpClient.DefaultRequestHeaders.Add("LWSSO_COOKIE_KEY", cookie.Value);
                }
            }

            _httpClient.DefaultRequestHeaders.Add("HPECLIENTTYPE", "HPE_MQM_UI");
            _currentUserName = user;
        }
        public async Task Logout()
        {
            var response = await _httpClient.PostAsync("/authentication/sign_out", null);
            response.EnsureSuccessStatusCode();
        }

        public void SetDefaultSharespace(int sharespaceId)
        {
            _defaultSharespaceId = sharespaceId;
        }
        public async Task SetDefaultWorkspace(Workspace workspace)
        {
            _defaultWorkspace = workspace;
            var response = await _httpClient.GetAsync($"api/shared_spaces/{_defaultSharespaceId}/workspaces/{_defaultWorkspace.id}/workspace_users?query=\"name='{_currentUserName}'\"");
            var result = await response.Content.ReadAsAsync<Users>();
            _currentUser = result.data[0];
        }

        public async Task<Workspaces> GetWorkspace()
        {
            var response = await _httpClient.GetAsync($"/api/shared_spaces/{_defaultSharespaceId}/workspaces");
            return await response.Content.ReadAsAsync<Workspaces>();
        }

        public async Task<Backlogs> GetBacklogs(string parameters = null)
        {
            var url = $"/api/shared_spaces/{_defaultSharespaceId}/workspaces/{_defaultWorkspace.id}/work_items";
            if (!string.IsNullOrEmpty(parameters)) url += "?" + parameters;
            var response = await _httpClient.GetAsync(url);
            return await response.Content.ReadAsAsync<Backlogs>();
        }
        public async Task<Backlogs> GetMyBacklogs()
        {
            var query = "owner={id=" + _currentUser.id + "};phase={metaphase={(name='New'||name='In Progress'||name='In Testing')}};(subtype='defect'||subtype='story'||subtype='quality_story')";            var expand = "$all{fields = name}";
            return await GetBacklogs(string.Format("query=\"{0}\"&expand={1}", query, expand));
        }

        public async Task<Tests> GetTests(string parameters = null)
        {
            var url = $"/api/shared_spaces/{_defaultSharespaceId}/workspaces/{_defaultWorkspace.id}/tests";
            if (!string.IsNullOrEmpty(parameters)) url += "?" + parameters;
            var response = await _httpClient.GetAsync(url);
            return await response.Content.ReadAsAsync<Tests>();
        }
        public async Task<Tests> GetMyTests()
        {
            var query = "owner={id=" + _currentUser.id + "};phase={metaphase={(name='New'||name='In Design')}};(subtype='test_manual'||subtype='gherkin_test')";            var expand = "$all{fields = name}";
            return await GetTests(string.Format("query=\"{0}\"&expand={1}", query, expand));
        }

        public async Task<Runs> GetRuns(string parameters = null)
        {
            var url = $"/api/shared_spaces/{_defaultSharespaceId}/workspaces/{_defaultWorkspace.id}/runs";
            if (!string.IsNullOrEmpty(parameters)) url += "?" + parameters;
            var response = await _httpClient.GetAsync(url);
            return await response.Content.ReadAsAsync<Runs>();
        }
        public async Task<Runs> GetMyRuns()
        {
            var query = "run_by={id=" + _currentUser.id + "};(native_status={id=2946}||native_status={id=2945}||native_status={id=2944});((parent_suite={null};subtype='run_manual')||(subtype='run_suite'))";            var expand = "$all{fields = name}";
            return await GetRuns(string.Format("query=\"{0}\"&skip_subtype_filter={1}&expand={2}", query, true, expand));
        }
    }
}