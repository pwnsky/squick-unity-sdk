// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: msg_id.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Rpc {

  /// <summary>Holder for reflection information generated from msg_id.proto</summary>
  public static partial class MsgIdReflection {

    #region Descriptor
    /// <summary>File descriptor for msg_id.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static MsgIdReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cgxtc2dfaWQucHJvdG8SA3JwYyrBAgoFTXNnSWQSDQoJTXNnSWRaZXJvEAAS",
            "FQoQSWRSZXFQbGF5ZXJFbnRlchDkXRIVChBJZEFja1BsYXllckVudGVyEOVd",
            "EhUKEElkUmVxUGxheWVyTGVhdmUQ5l0SFQoQSWRBY2tQbGF5ZXJMZWF2ZRDn",
            "XRIUCg9JZFJlcVBsYXllckRhdGEQ6F0SFAoPSWRBY2tQbGF5ZXJEYXRhEOld",
            "EhYKEUlkUmVxQ29ubmVjdFByb3h5EMM+EhYKEUlkQWNrQ29ubmVjdFByb3h5",
            "EMQ+EhkKFElkUmVxRGlzY29ubmVjdFByb3h5EMU+EhkKFElkQWNrRGlzY29u",
            "bmVjdFByb3h5EMc+EhMKDklkUmVxSGVhcnRCZWF0EMg+EhMKDklkQWNrSGVh",
            "cnRCZWF0EMk+EhEKDElkQWNrS2lja09mZhDKPmIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Rpc.MsgId), }, null, null));
    }
    #endregion

  }
  #region Enums
  public enum MsgId {
    [pbr::OriginalName("MsgIdZero")] Zero = 0,
    /// <summary>
    /// message: rpc.ReqPlayerEnter desc: 
    /// </summary>
    [pbr::OriginalName("IdReqPlayerEnter")] IdReqPlayerEnter = 12004,
    /// <summary>
    /// message: rpc.AckPlayerEnter desc: 
    /// </summary>
    [pbr::OriginalName("IdAckPlayerEnter")] IdAckPlayerEnter = 12005,
    /// <summary>
    /// message: rpc.ReqPlayerLeave desc: 
    /// </summary>
    [pbr::OriginalName("IdReqPlayerLeave")] IdReqPlayerLeave = 12006,
    /// <summary>
    /// message: rpc.AckPlayerLeave desc: 
    /// </summary>
    [pbr::OriginalName("IdAckPlayerLeave")] IdAckPlayerLeave = 12007,
    /// <summary>
    /// message: rpc.ReqPlayerData desc: 
    /// </summary>
    [pbr::OriginalName("IdReqPlayerData")] IdReqPlayerData = 12008,
    /// <summary>
    /// message: rpc.AckPlayerData desc: 
    /// </summary>
    [pbr::OriginalName("IdAckPlayerData")] IdAckPlayerData = 12009,
    /// <summary>
    /// message: rpc.ReqConnectProxy desc: Request to connect the proxy
    /// </summary>
    [pbr::OriginalName("IdReqConnectProxy")] IdReqConnectProxy = 8003,
    /// <summary>
    /// message: rpc.AckConnectProxy desc: Request to connect the proxy response
    /// </summary>
    [pbr::OriginalName("IdAckConnectProxy")] IdAckConnectProxy = 8004,
    /// <summary>
    /// message: rpc.ReqDisconnectProxy desc: 
    /// </summary>
    [pbr::OriginalName("IdReqDisconnectProxy")] IdReqDisconnectProxy = 8005,
    /// <summary>
    /// message: rpc.AckDisconnectProxy desc: 
    /// </summary>
    [pbr::OriginalName("IdAckDisconnectProxy")] IdAckDisconnectProxy = 8007,
    /// <summary>
    /// message: rpc.ReqHeartBeat desc: 
    /// </summary>
    [pbr::OriginalName("IdReqHeartBeat")] IdReqHeartBeat = 8008,
    /// <summary>
    /// message: rpc.AckHeartBeat desc: 
    /// </summary>
    [pbr::OriginalName("IdAckHeartBeat")] IdAckHeartBeat = 8009,
    /// <summary>
    /// message: rpc.AckKickOff desc: 
    /// </summary>
    [pbr::OriginalName("IdAckKickOff")] IdAckKickOff = 8010,
  }

  #endregion

}

#endregion Designer generated code
