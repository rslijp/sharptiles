<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SharpTilesSample._Default" %>

<script runat="server">
  protected override void OnLoad(EventArgs e)
  {
    Response.Redirect("~/home/index.rails");
    base.OnLoad(e);
  }
</script>