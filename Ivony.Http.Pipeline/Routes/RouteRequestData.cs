﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Http.Pipeline.Routes
{
  /// <summary>
  /// contains request data for use by the routing system.
  /// </summary>
  public sealed class RouteRequestData
  {


    /// <summary>
    /// create RouteRequestData instance.
    /// </summary>
    /// <param name="request">request message</param>
    public RouteRequestData( HttpRequestMessage request )
    {
      Path = PathSegments.Create( request.RequestUri.AbsolutePath );
      Host = HostSegments.Create( request.RequestUri.Host );
      Scheme = request.RequestUri.Scheme;
      QueryString = new ReadOnlyCollection<(string name, string value)>( ParseQueryString( request.RequestUri.Query ).ToArray() );
    }


    private IEnumerable<(string name, string value)> ParseQueryString( string query )
    {
      if ( query == null )
        return null;


      var list = new List<(string name, string value)>();
      var name = new List<char>( 20 );
      var value = new List<char>( 50 );

      bool status = false;
      bool hasValue = false;


      for ( int i = 1; i < query.Length; i++ )
      {

        char ch = query[i];
        if ( ch == '=' )
        {
          if ( name == null )
            throw new FormatException();

          status = true;
          hasValue = true;
        }
        else if ( ch == '&' )
        {
          var n = new string( name.ToArray() );
          if ( hasValue )
            list.Add( (n, new string( value.ToArray() )) );

          else
            list.Add( (n, null) );

          status = false;
          hasValue = false;

          name = new List<char>( 20 );
          value = new List<char>( 50 );
        }
        else if ( status == false )
        {
          name.Add( ch );
        }
        else
        {
          value.Add( ch );
        }
      }

      return list;
    }

    /// <summary>
    /// request scheme
    /// </summary>
    public string Scheme { get; }

    /// <summary>
    ///  absoluate path segments
    /// </summary>
    public PathSegments Path { get; }

    /// <summary>
    /// query string
    /// </summary>
    public IReadOnlyList<(string name, string value)> QueryString { get; }

    /// <summary>
    /// HTTP headers
    /// </summary>
    public IReadOnlyDictionary<string, string> Headers { get; }

    /// <summary>
    /// host segments
    /// </summary>
    public HostSegments Host { get; }


    /// <summary>
    /// port information
    /// </summary>
    public int? Port { get; }

  }
}
