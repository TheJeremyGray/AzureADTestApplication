<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Mas20TestApplication._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>ASP.NET</h1>
        <p class="lead">ASP.NET is a free web framework for building great Web sites and Web applications using HTML, CSS, and JavaScript.</p>
        <p><a href="http://www.asp.net" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
    </div>

    <div class="row">
        <div class="col-md-12">
            <h2>Claims</h2>
            
            <table>
                 <tr>
                    <th>Claim Type</th>
                    <th>Claim Value</th>
                </tr>
              <% foreach (System.Security.Claims.Claim claim in System.Security.Claims.ClaimsPrincipal.Current.Claims) { %>
                <tr>
                    <td><%= claim.Type %></td>
                    <td><%= claim.Value %></td>
                </tr>
              <% } %>
            </table>

        </div>    
    </div>

</asp:Content>
